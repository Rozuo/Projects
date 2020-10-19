--[[
    GD50
    Breakout Remake

    -- StartState Class --

    Author: Colton Ogden
    cogden@cs50.harvard.edu

    Represents the state that the game is in when we've just completed a level.
    Very similar to the ServeState, except here we increment the level 
]]

VictoryState = Class{__includes = BaseState}

function VictoryState:enter(params)
    self.level = params.level
    self.score = params.score
    self.highScores = params.highScores
    self.paddle = params.paddle
    self.health = params.health
    self.ball1 = params.ball
    self.recoverPoints = params.recoverPoints
    if(self.ball1 == nil) then
        self.ball1 = Ball()
        self.ball1.skin = math.random(7)
    end
end

function VictoryState:update(dt)
    self.paddle:update(dt)

    -- have the ball track the player
    self.ball1.x = self.paddle.x + (self.paddle.width / 2) - 4
    self.ball1.y = self.paddle.y - 8

    -- go to play screen if the player presses Enter
    if love.keyboard.wasPressed('enter') or love.keyboard.wasPressed('return') then
        local level = LevelMaker.createMap(self.level + 1)
        local keyPowerUp = nil
        -- determine if we need a key powerup
        for k, brick in pairs(level) do
            if(brick.locked) then
                keyPowerUp = true
                break
            else
                keyPowerUp = false
            end
        end

        gStateMachine:change('serve', {
            level = self.level + 1,
            bricks = level, --LevelMaker.createMap(self.level + 1),
            needKeyPowerUp = keyPowerUp,
            paddle = self.paddle,
            health = self.health,
            score = self.score,
            highScores = self.highScores,
            recoverPoints = self.recoverPoints
        })
    end
end

function VictoryState:render()
    self.paddle:render()
    self.ball1:render()

    renderHealth(self.health)
    renderScore(self.score)

    -- level complete text
    love.graphics.setFont(gFonts['large'])
    love.graphics.printf("Level " .. tostring(self.level) .. " complete!",
        0, VIRTUAL_HEIGHT / 4, VIRTUAL_WIDTH, 'center')

    -- instructions text
    love.graphics.setFont(gFonts['medium'])
    love.graphics.printf('Press Enter to serve!', 0, VIRTUAL_HEIGHT / 2,
        VIRTUAL_WIDTH, 'center')
end