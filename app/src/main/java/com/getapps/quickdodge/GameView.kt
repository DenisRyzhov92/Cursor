package com.getapps.quickdodge

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.os.Handler
import android.os.Looper
import android.os.SystemClock
import android.view.MotionEvent
import android.view.View
import kotlin.math.max
import kotlin.math.min
import kotlin.math.pow
import kotlin.random.Random

class GameView(context: Context) : View(context), Runnable {
    private enum class GameState {
        READY,
        PLAYING,
        GAME_OVER,
    }

    private data class Obstacle(
        var x: Float,
        var y: Float,
        var size: Float,
        var speed: Float,
    )

    private val frameHandler = Handler(Looper.getMainLooper())
    private val rng = Random(SystemClock.elapsedRealtimeNanos())
    private val prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)

    private val backgroundPaint = Paint().apply { color = Color.parseColor("#111218") }
    private val playerPaint = Paint(Paint.ANTI_ALIAS_FLAG).apply { color = Color.parseColor("#2AE7FF") }
    private val obstaclePaint = Paint(Paint.ANTI_ALIAS_FLAG).apply { color = Color.parseColor("#FF4D6D") }
    private val titlePaint = Paint(Paint.ANTI_ALIAS_FLAG).apply {
        color = Color.WHITE
        textAlign = Paint.Align.CENTER
    }
    private val bodyPaint = Paint(Paint.ANTI_ALIAS_FLAG).apply {
        color = Color.parseColor("#D7DBE3")
        textAlign = Paint.Align.CENTER
    }
    private val hudPaint = Paint(Paint.ANTI_ALIAS_FLAG).apply {
        color = Color.WHITE
        textAlign = Paint.Align.LEFT
    }

    private val obstacles = mutableListOf<Obstacle>()
    private var state = GameState.READY

    private var running = false
    private var lastFrameNs = 0L
    private var lastSpawnMs = 0L
    private var scoreProgress = 0f
    private var score = 0
    private var bestScore = prefs.getInt(KEY_BEST_SCORE, 0)

    private var playerX = 0f
    private var playerY = 0f
    private var playerRadius = 0f

    override fun onAttachedToWindow() {
        super.onAttachedToWindow()
        running = true
        frameHandler.post(this)
    }

    override fun onDetachedFromWindow() {
        super.onDetachedFromWindow()
        running = false
        frameHandler.removeCallbacks(this)
    }

    override fun onSizeChanged(w: Int, h: Int, oldw: Int, oldh: Int) {
        super.onSizeChanged(w, h, oldw, oldh)
        if (w == 0 || h == 0) return
        playerRadius = min(w, h) * 0.045f
        playerX = w * 0.5f
        playerY = h * 0.86f
        titlePaint.textSize = w * 0.12f
        bodyPaint.textSize = w * 0.055f
        hudPaint.textSize = w * 0.055f
    }

    override fun run() {
        val nowNs = SystemClock.elapsedRealtimeNanos()
        if (lastFrameNs == 0L) lastFrameNs = nowNs
        val dtSeconds = ((nowNs - lastFrameNs) / 1_000_000_000f).coerceAtMost(0.05f)
        lastFrameNs = nowNs

        if (state == GameState.PLAYING) {
            update(dtSeconds)
        }
        invalidate()

        if (running) {
            frameHandler.postDelayed(this, FRAME_DELAY_MS)
        }
    }

    override fun onTouchEvent(event: MotionEvent): Boolean {
        when (event.actionMasked) {
            MotionEvent.ACTION_DOWN -> {
                when (state) {
                    GameState.READY, GameState.GAME_OVER -> startRun()
                    GameState.PLAYING -> movePlayer(event.x)
                }
            }

            MotionEvent.ACTION_MOVE -> {
                if (state == GameState.PLAYING) {
                    movePlayer(event.x)
                }
            }
        }
        return true
    }

    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)
        canvas.drawRect(0f, 0f, width.toFloat(), height.toFloat(), backgroundPaint)

        for (obstacle in obstacles) {
            canvas.drawRect(
                obstacle.x - obstacle.size,
                obstacle.y - obstacle.size,
                obstacle.x + obstacle.size,
                obstacle.y + obstacle.size,
                obstaclePaint,
            )
        }

        canvas.drawCircle(playerX, playerY, playerRadius, playerPaint)
        canvas.drawText("Score: $score", width * 0.04f, hudPaint.textSize * 1.3f, hudPaint)
        canvas.drawText("Best: $bestScore", width * 0.04f, hudPaint.textSize * 2.5f, hudPaint)

        when (state) {
            GameState.READY -> {
                canvas.drawText("Quick Dodge", width * 0.5f, height * 0.38f, titlePaint)
                canvas.drawText("Tap to start", width * 0.5f, height * 0.48f, bodyPaint)
                canvas.drawText("Move finger left/right", width * 0.5f, height * 0.54f, bodyPaint)
            }

            GameState.GAME_OVER -> {
                canvas.drawText("Game Over", width * 0.5f, height * 0.40f, titlePaint)
                canvas.drawText("Run score: $score", width * 0.5f, height * 0.50f, bodyPaint)
                canvas.drawText("Tap to restart", width * 0.5f, height * 0.56f, bodyPaint)
            }

            GameState.PLAYING -> Unit
        }
    }

    private fun update(dt: Float) {
        scoreProgress += dt * SCORE_PER_SECOND
        score = scoreProgress.toInt()

        val speedMultiplier = 1f + score * 0.015f
        val spawnIntervalMs = max(MIN_SPAWN_INTERVAL_MS, BASE_SPAWN_INTERVAL_MS - score * 3L)
        val nowMs = SystemClock.elapsedRealtime()

        if (nowMs - lastSpawnMs >= spawnIntervalMs) {
            spawnObstacle()
            lastSpawnMs = nowMs
        }

        var collided = false
        for (obstacle in obstacles) {
            obstacle.y += obstacle.speed * speedMultiplier * dt
            if (isPlayerHit(obstacle)) {
                collided = true
                break
            }
        }

        obstacles.removeAll { it.y - it.size > height + OFFSCREEN_MARGIN_PX }

        if (collided) {
            endRun()
        }
    }

    private fun startRun() {
        obstacles.clear()
        state = GameState.PLAYING
        score = 0
        scoreProgress = 0f
        playerX = width * 0.5f
        lastSpawnMs = 0L
        lastFrameNs = 0L
    }

    private fun endRun() {
        state = GameState.GAME_OVER
        if (score > bestScore) {
            bestScore = score
            prefs.edit().putInt(KEY_BEST_SCORE, bestScore).apply()
        }
    }

    private fun movePlayer(rawX: Float) {
        val minX = playerRadius
        val maxX = width - playerRadius
        playerX = rawX.coerceIn(minX, maxX)
    }

    private fun spawnObstacle() {
        if (width == 0 || height == 0) return
        val size = randomIn(width * 0.05f, width * 0.11f)
        val minX = size
        val maxX = width - size
        obstacles += Obstacle(
            x = randomIn(minX, maxX),
            y = -size,
            size = size,
            speed = randomIn(360f, 620f),
        )
    }

    private fun isPlayerHit(obstacle: Obstacle): Boolean {
        val closestX = playerX.coerceIn(obstacle.x - obstacle.size, obstacle.x + obstacle.size)
        val closestY = playerY.coerceIn(obstacle.y - obstacle.size, obstacle.y + obstacle.size)
        val dx = playerX - closestX
        val dy = playerY - closestY
        return dx.pow(2) + dy.pow(2) <= playerRadius.pow(2)
    }

    private fun randomIn(minValue: Float, maxValue: Float): Float {
        if (maxValue <= minValue) return minValue
        return minValue + (maxValue - minValue) * rng.nextFloat()
    }

    private companion object {
        private const val PREFS_NAME = "quick_dodge"
        private const val KEY_BEST_SCORE = "best_score"
        private const val FRAME_DELAY_MS = 16L
        private const val SCORE_PER_SECOND = 10f
        private const val BASE_SPAWN_INTERVAL_MS = 700L
        private const val MIN_SPAWN_INTERVAL_MS = 260L
        private const val OFFSCREEN_MARGIN_PX = 80f
    }
}
