--[[
    GD50
    Breakout Remake

    -- PlayState Class --

    Author: Colton Ogden
    cogden@cs50.harvard.edu

    Represents the state of the game in which we are actively playing;
    player should control the paddle, with the ball actively bouncing between
    the bricks, walls, and the paddle. If the ball goes below the paddle, then
    the player should lose one point of health and be taken either to the Game
    Over screen if at 0 health or the Serve screen otherwise.
]]

PlayState = Class{__includes = BaseState}

--[[
    We initialize what's in our PlayState via a state table that we pass between
    states as we go from playing to serving.
]]
function PlayState:enter(params)
    self.paddle = params.paddle
    self.bricks = params.bricks
    self.health = params.health
    self.score = params.score
    self.highScores = params.highScores
    self.needKeyPowerUp = params.needKeyPowerUp
    
    self.ball1 = params.ball1

    self.ball2 = params.ball2 or nil
    self.ball3 = params.ball3 or nil

    self.level = params.level

    self.recoverPoints = 5000

    self.growPoints = 1000

    self.powerUp = Powerup()
    self.powerUpSpawnTime = math.random(4, 8)
    self.powerUpTimer = 0


    if(self.needKeyPowerUp) then
        self.keyPowerUp = Powerup()
        self.keyPowerUp.texture = gFrames['powerups'][10]
        self.keyPowerUp.dy = self.keyPowerUp.dy*2
        self.hasKey = false
        self.keyPowerUpSpawnTime = math.random(2, 8)
        self.keyTimer = 0
    end
    

    -- give ball random starting velocity
    self.ball1.dx = math.random(-200, 200)
    self.ball1.dy = math.random(-50, -60)
end

function PlayState:update(dt)
    
    self.powerUpTimer = self.powerUpTimer + dt
    

    if(self.powerUpTimer >= self.powerUpSpawnTime and not(self.powerUp.inPlay)) then
        self.powerUp = Powerup()
        self.powerUp.inPlay = true
    end
    if(self.needKeyPowerUp) then
        self.keyTimer = self.keyTimer + dt
        if(self.keyTimer >= self.keyPowerUpSpawnTime and not(self.keyPowerUp.inPlay) and not(self.hasKey)) then
            self.keyPowerUp = Powerup()
            self.keyPowerUp.texture = gFrames['powerups'][10]
            self.keyPowerUp.dy = self.keyPowerUp.dy*2
            self.keyPowerUp.inPlay = true
        end
    end

    if self.paused then
        if love.keyboard.wasPressed('space') then
            self.paused = false
            gSounds['pause']:play()
        else
            return
        end
    elseif love.keyboard.wasPressed('space') then
        self.paused = true
        gSounds['pause']:play()
        return
    end

    -- update positions based on velocity
    self.paddle:update(dt)
    if(self.ball1 ~= nil) then
        self.ball1:update(dt)
    end
    if(self.ball2 ~= nil) then
        self.ball2:update(dt)
    end
    if(self.ball3 ~= nil) then
        self.ball3:update(dt)
    end

    

    if(self.powerUp.inPlay) then
        self.powerUp:update(dt)
    end

    if(self.needKeyPowerUp and self.keyPowerUp.inPlay) then
        self.keyPowerUp:update(dt)
    end

    if self.ball1 ~= nil and self.ball1:collides(self.paddle) then
        -- raise ball above paddle in case it goes below it, then reverse dy
        self.ball1.y = self.paddle.y - 8
        self.ball1.dy = -self.ball1.dy

        --
        -- tweak angle of bounce based on where it hits the paddle
        --

        -- if we hit the paddle on its left side while moving left...
        if self.ball1.x < self.paddle.x + (self.paddle.width / 2) and self.paddle.dx < 0 then
            self.ball1.dx = -50 + -(8 * (self.paddle.x + self.paddle.width / 2 - self.ball1.x))
        
        -- else if we hit the paddle on its right side while moving right...
        elseif self.ball1.x > self.paddle.x + (self.paddle.width / 2) and self.paddle.dx > 0 then
            self.ball1.dx = 50 + (8 * math.abs(self.paddle.x + self.paddle.width / 2 - self.ball1.x))
        end

        gSounds['paddle-hit']:play()
    end

    if self.ball2 ~= nil and self.ball2:collides(self.paddle) then
        -- raise ball above paddle in case it goes below it, then reverse dy
        self.ball2.y = self.paddle.y - 8
        self.ball2.dy = -self.ball2.dy

        --
        -- tweak angle of bounce based on where it hits the paddle
        --

        -- if we hit the paddle on its left side while moving left...
        if self.ball2.x < self.paddle.x + (self.paddle.width / 2) and self.paddle.dx < 0 then
            self.ball2.dx = -50 + -(8 * (self.paddle.x + self.paddle.width / 2 - self.ball2.x))
        
        -- else if we hit the paddle on its right side while moving right...
        elseif self.ball2.x > self.paddle.x + (self.paddle.width / 2) and self.paddle.dx > 0 then
            self.ball2.dx = 50 + (8 * math.abs(self.paddle.x + self.paddle.width / 2 - self.ball2.x))
        end

        gSounds['paddle-hit']:play()
    end

    if self.ball3 ~= nil and self.ball3:collides(self.paddle) then
        -- raise ball above paddle in case it goes below it, then reverse dy
        self.ball3.y = self.paddle.y - 8
        self.ball3.dy = -self.ball3.dy

        --
        -- tweak angle of bounce based on where it hits the paddle
        --

        -- if we hit the paddle on its left side while moving left...
        if self.ball3.x < self.paddle.x + (self.paddle.width / 2) and self.paddle.dx < 0 then
            self.ball3.dx = -50 + -(8 * (self.paddle.x + self.paddle.width / 2 - self.ball3.x))
        
        -- else if we hit the paddle on its right side while moving right...
        elseif self.ball3.x > self.paddle.x + (self.paddle.width / 2) and self.paddle.dx > 0 then
            self.ball3.dx = 50 + (8 * math.abs(self.paddle.x + self.paddle.width / 2 - self.ball3.x))
        end

        gSounds['paddle-hit']:play()
    end

    -- if the paddle collides with the powerup icon create two new balls and put them in play immediately.
    if self.powerUp.inPlay and self.powerUp:collides(self.paddle) then
        if(self.ball1 == nil) then
            self.ball1 = Ball()
            self.ball1.skin = math.random(7)

            self.ball1.x = self.paddle.x + (self.paddle.width / 2) - 4
            self.ball1.y = self.paddle.y - 8
            
            self.ball1.dx = math.random(-200, 200)
            self.ball1.dy = math.random(-50, -60)
        end
        if(self.ball2 == nil) then
            self.ball2 = Ball()
            self.ball2.skin = math.random(7)

            self.ball2.x = self.paddle.x + (self.paddle.width / 2) - 4
            self.ball2.y = self.paddle.y - 8
            
            self.ball2.dx = math.random(-200, 200)
            self.ball2.dy = math.random(-50, -60)
        end
        if(self.ball3 == nil) then
            self.ball3 = Ball()
            self.ball3.skin = math.random(7)

            self.ball3.x = self.paddle.x + (self.paddle.width / 2) - 4
            self.ball3.y = self.paddle.y - 8
            
            self.ball3.dx = math.random(-200, 200)
            self.ball3.dy = math.random(-50, -60)

            
        end
        gSounds['powerup']:play()
        self.powerUp.inPlay = false
        self.powerUpTimer = 0
        self.powerUpSpawnTime = math.random(4, 8)
    end


    if self.needKeyPowerUp and self.keyPowerUp.inPlay and self.keyPowerUp:collides(self.paddle) then
        self.hasKey = true
        self.keyPowerUp.inPlay = false
        gSounds['key']:play()
    end


    if(self.ball1 ~= nil) then
        -- detect collision across all bricks with the ball
        for k, brick in pairs(self.bricks) do

            -- only check collision if we're in play
            if brick.inPlay and self.ball1:collides(brick) then
                if(brick.locked and self.hasKey) then
                    -- add to score
                    self.score = self.score + (10 * 200 + 10 * 25)

                    -- trigger the brick's hit function, which removes it from play
                    brick:hit()

                    -- if we have enough points, recover a point of health
                    if self.score > self.recoverPoints then
                        -- can't go above 3 health
                        self.health = math.min(3, self.health + 1)

                        -- multiply recover points by 2
                        self.recoverPoints = math.min(100000, self.recoverPoints * 2)

                        -- play recover sound effect
                        gSounds['recover']:play()
                    end


                    -- set the paddle growth to a increase 1000 points
                    if(self.score > self.growPoints) then
                        if(self.paddle.size ~= 4) then
                            self.paddle.size = self.paddle.size + 1
                            self.paddle:changeDimensions()
                        end
                        self.growPoints = self.growPoints + 6000
                        gSounds['grow']:play()
                    end
                elseif(not(brick.locked)) then
                   -- add to score
                    self.score = self.score + (brick.tier * 200 + brick.color * 25)

                    -- trigger the brick's hit function, which removes it from play
                    brick:hit()

                    -- if we have enough points, recover a point of health
                    if self.score > self.recoverPoints then
                        -- can't go above 3 health
                        self.health = math.min(3, self.health + 1)

                        -- multiply recover points by 2
                        self.recoverPoints = math.min(100000, self.recoverPoints * 2)

                        -- play recover sound effect
                        gSounds['recover']:play()
                    end


                    -- set the paddle growth to a increase 1000 points
                    if(self.score > self.growPoints) then
                        if(self.paddle.size ~= 4) then
                            self.paddle.size = self.paddle.size + 1
                            self.paddle:changeDimensions()
                        end
                        self.growPoints = self.growPoints + 6000
                        gSounds['grow']:play()
                    end
                end

                -- go to our victory screen if there are no more bricks left
                if self:checkVictory() then
                    gSounds['victory']:play()

                    gStateMachine:change('victory', {
                        level = self.level,
                        needKeyPowerUp = self.needKeyPowerUp,
                        paddle = self.paddle,
                        health = self.health,
                        score = self.score,
                        highScores = self.highScores,
                        ball1 = self.ball1,
                        ball2 = self.ball2,
                        ball3 = self.ball3,
                        recoverPoints = self.recoverPoints
                    })
                end

                --
                -- collision code for bricks
                --
                -- we check to see if the opposite side of our velocity is outside of the brick;
                -- if it is, we trigger a collision on that side. else we're within the X + width of
                -- the brick and should check to see if the top or bottom edge is outside of the brick,
                -- colliding on the top or bottom accordingly 
                --

                -- left edge; only check if we're moving right, and offset the check by a couple of pixels
                -- so that flush corner hits register as Y flips, not X flips
                if self.ball1.x + 2 < brick.x and self.ball1.dx > 0 then
                    
                    -- flip x velocity and reset position outside of brick
                    self.ball1.dx = -self.ball1.dx
                    self.ball1.x = brick.x - 8
                
                -- right edge; only check if we're moving left, , and offset the check by a couple of pixels
                -- so that flush corner hits register as Y flips, not X flips
                elseif self.ball1.x + 6 > brick.x + brick.width and self.ball1.dx < 0 then
                    
                    -- flip x velocity and reset position outside of brick
                    self.ball1.dx = -self.ball1.dx
                    self.ball1.x = brick.x + 32
                
                -- top edge if no X collisions, always check
                elseif self.ball1.y < brick.y then
                    
                    -- flip y velocity and reset position outside of brick
                    self.ball1.dy = -self.ball1.dy
                    self.ball1.y = brick.y - 8
                
                -- bottom edge if no X collisions or top collision, last possibility
                else
                    
                    -- flip y velocity and reset position outside of brick
                    self.ball1.dy = -self.ball1.dy
                    self.ball1.y = brick.y + 16
                end

                -- slightly scale the y velocity to speed up the game, capping at +- 150
                if math.abs(self.ball1.dy) < 150 then
                    self.ball1.dy = self.ball1.dy * 1.02
                end
                -- only allow colliding with one brick, for corners
                break
            end
        end
    end

    if (self.ball2 ~= nil) then
        -- detect collision across all bricks with the ball
        for k, brick in pairs(self.bricks) do

            -- only check collision if we're in play
            if brick.inPlay and self.ball2:collides(brick) then
                if(brick.locked and self.hasKey) then

                    -- add to score
                    self.score = self.score + (10 * 200 + 10 * 25)

                    -- trigger the brick's hit function, which removes it from play
                    brick:hit()

                    -- if we have enough points, recover a point of health
                    if self.score > self.recoverPoints then
                        -- can't go above 3 health
                        self.health = math.min(3, self.health + 1)

                        -- multiply recover points by 2
                        self.recoverPoints = math.min(100000, self.recoverPoints * 2)

                        -- play recover sound effect
                        gSounds['recover']:play()
                    end

                    -- set the paddle growth to a increase 1000 points
                    if(self.score > self.growPoints) then
                        if(self.paddle.size ~= 4) then
                            self.paddle.size = self.paddle.size + 1
                            self.paddle:changeDimensions()
                        end
                        self.growPoints = self.growPoints + 6000
                        gSounds['grow']:play()
                    end
                elseif(not(brick.locked)) then
                    -- add to score
                    self.score = self.score + (brick.tier * 200 + brick.color * 25)

                    -- trigger the brick's hit function, which removes it from play
                    brick:hit()

                    -- if we have enough points, recover a point of health
                    if self.score > self.recoverPoints then
                        -- can't go above 3 health
                        self.health = math.min(3, self.health + 1)

                        -- multiply recover points by 2
                        self.recoverPoints = math.min(100000, self.recoverPoints * 2)

                        -- play recover sound effect
                        gSounds['recover']:play()
                    end

                    -- set the paddle growth to a increase 1000 points
                    if(self.score > self.growPoints) then
                        if(self.paddle.size ~= 4) then
                            self.paddle.size = self.paddle.size + 1
                            self.paddle:changeDimensions()
                        end
                        self.growPoints = self.growPoints + 6000
                        gSounds['grow']:play()
                    end
                end



                -- go to our victory screen if there are no more bricks left
                if self:checkVictory() then
                    gSounds['victory']:play()

                    gStateMachine:change('victory', {
                        level = self.level,
                        needKeyPowerUp = self.needKeyPowerUp,
                        paddle = self.paddle,
                        health = self.health,
                        score = self.score,
                        highScores = self.highScores,
                        ball1 = self.ball1,
                        ball2 = self.ball2,
                        ball3 = self.ball3,
                        recoverPoints = self.recoverPoints
                    })
                end

                --
                -- collision code for bricks
                --
                -- we check to see if the opposite side of our velocity is outside of the brick;
                -- if it is, we trigger a collision on that side. else we're within the X + width of
                -- the brick and should check to see if the top or bottom edge is outside of the brick,
                -- colliding on the top or bottom accordingly 
                --

                -- left edge; only check if we're moving right, and offset the check by a couple of pixels
                -- so that flush corner hits register as Y flips, not X flips
                if self.ball2.x + 2 < brick.x and self.ball2.dx > 0 then
                    
                    -- flip x velocity and reset position outside of brick
                    self.ball2.dx = -self.ball2.dx
                    self.ball2.x = brick.x - 8
                
                -- right edge; only check if we're moving left, , and offset the check by a couple of pixels
                -- so that flush corner hits register as Y flips, not X flips
                elseif self.ball2.x + 6 > brick.x + brick.width and self.ball2.dx < 0 then
                    
                    -- flip x velocity and reset position outside of brick
                    self.ball2.dx = -self.ball2.dx
                    self.ball2.x = brick.x + 32
                
                -- top edge if no X collisions, always check
                elseif self.ball2.y < brick.y then
                    
                    -- flip y velocity and reset position outside of brick
                    self.ball2.dy = -self.ball2.dy
                    self.ball2.y = brick.y - 8
                
                -- bottom edge if no X collisions or top collision, last possibility
                else
                    
                    -- flip y velocity and reset position outside of brick
                    self.ball2.dy = -self.ball2.dy
                    self.ball2.y = brick.y + 16
                end

                -- slightly scale the y velocity to speed up the game, capping at +- 150
                if math.abs(self.ball2.dy) < 150 then
                    self.ball2.dy = self.ball2.dy * 1.02
                end
                -- only allow colliding with one brick, for corners
                break
            end
        end
    end

    if (self.ball3 ~= nil) then
        -- detect collision across all bricks with the ball
        for k, brick in pairs(self.bricks) do

            -- only check collision if we're in play
            if brick.inPlay and self.ball3:collides(brick) then
                if(brick.locked and self.hasKey) then
                    -- add to score
                    self.score = self.score + (10 * 200 + 10 * 25)

                    -- trigger the brick's hit function, which removes it from play
                    brick:hit()

                    -- if we have enough points, recover a point of health
                    if self.score > self.recoverPoints then
                        -- can't go above 3 health
                        self.health = math.min(3, self.health + 1)

                        -- multiply recover points by 2
                        self.recoverPoints = math.min(100000, self.recoverPoints * 2)

                        -- play recover sound effect
                        gSounds['recover']:play()
                    end
                    
                    -- set the paddle growth to a increase 1000 points
                    if(self.score > self.growPoints) then
                        if(self.paddle.size ~= 4) then
                            self.paddle.size = self.paddle.size + 1
                            self.paddle:changeDimensions()
                        end
                        self.growPoints = self.growPoints + 6000
                        gSounds['grow']:play()
                    end
                elseif(not(brick.locked)) then
                    -- add to score
                    self.score = self.score + (brick.tier * 200 + brick.color * 25)

                    -- trigger the brick's hit function, which removes it from play
                    brick:hit()

                    -- if we have enough points, recover a point of health
                    if self.score > self.recoverPoints then
                        -- can't go above 3 health
                        self.health = math.min(3, self.health + 1)

                        -- multiply recover points by 2
                        self.recoverPoints = math.min(100000, self.recoverPoints * 2)

                        -- play recover sound effect
                        gSounds['recover']:play()
                    end
                    
                    -- set the paddle growth to a increase 1000 points
                    if(self.score > self.growPoints) then
                        if(self.paddle.size ~= 4) then
                            self.paddle.size = self.paddle.size + 1
                            self.paddle:changeDimensions()
                        end
                        self.growPoints = self.growPoints + 6000
                        gSounds['grow']:play()
                    end
                end

                -- go to our victory screen if there are no more bricks left
                if self:checkVictory() then
                    gSounds['victory']:play()

                    gStateMachine:change('victory', {
                        level = self.level,
                        needKeyPowerUp = self.needKeyPowerUp,
                        paddle = self.paddle,
                        health = self.health,
                        score = self.score,
                        highScores = self.highScores,
                        ball1 = self.ball1,
                        ball2 = self.ball2,
                        ball3 = self.ball3,
                        recoverPoints = self.recoverPoints
                    })
                end

                --
                -- collision code for bricks
                --
                -- we check to see if the opposite side of our velocity is outside of the brick;
                -- if it is, we trigger a collision on that side. else we're within the X + width of
                -- the brick and should check to see if the top or bottom edge is outside of the brick,
                -- colliding on the top or bottom accordingly 
                --

                -- left edge; only check if we're moving right, and offset the check by a couple of pixels
                -- so that flush corner hits register as Y flips, not X flips
                if self.ball3.x + 2 < brick.x and self.ball3.dx > 0 then
                    
                    -- flip x velocity and reset position outside of brick
                    self.ball3.dx = -self.ball3.dx
                    self.ball3.x = brick.x - 8
                
                -- right edge; only check if we're moving left, , and offset the check by a couple of pixels
                -- so that flush corner hits register as Y flips, not X flips
                elseif self.ball3.x + 6 > brick.x + brick.width and self.ball3.dx < 0 then
                    
                    -- flip x velocity and reset position outside of brick
                    self.ball3.dx = -self.ball3.dx
                    self.ball3.x = brick.x + 32
                
                -- top edge if no X collisions, always check
                elseif self.ball3.y < brick.y then
                    
                    -- flip y velocity and reset position outside of brick
                    self.ball3.dy = -self.ball3.dy
                    self.ball3.y = brick.y - 8
                
                -- bottom edge if no X collisions or top collision, last possibility
                else
                    
                    -- flip y velocity and reset position outside of brick
                    self.ball3.dy = -self.ball3.dy
                    self.ball3.y = brick.y + 16
                end

                -- slightly scale the y velocity to speed up the game, capping at +- 150
                if math.abs(self.ball3.dy) < 150 then
                    self.ball3.dy = self.ball3.dy * 1.02
                end
                -- only allow colliding with one brick, for corners
                break
            end
        end
    end

    -- if ball goes below bounds, revert to serve state and decrease health
    if  self.ball1 ~= nil and self.ball1.y >= VIRTUAL_HEIGHT  then
        self.ball1 = nil
        --self.health = self.health - 1
        --gSounds['hurt']:play()

    end
    if self.ball2 ~= nil and self.ball2.y >= VIRTUAL_HEIGHT then
        self.ball2 = nil
    end
    if self.ball3 ~= nil and self.ball3.y >= VIRTUAL_HEIGHT  then
        self.ball3 = nil
    end

    if(self.powerUp.y >= VIRTUAL_HEIGHT and self.powerUp.inPlay) then
        self.powerUp.inPlay = false
        self.timer = 0
        self.powerUpSpawnTime = math.random(4, 8)
    end

    if(self.needKeyPowerUp and self.keyPowerUp.y >= VIRTUAL_HEIGHT and self.keyPowerUp.inPlay) then
        self.keyPowerUp.inPlay = false
        self.keyTimer = 0
        self.keyPowerUpSpawnTime = math.random(2, 8)
    end

    if(self.ball1 == nil and self.ball2 == nil and  self.ball3 == nil) then
        self.health = self.health - 1
        gSounds['hurt']:play()
        
        if self.health == 0 then
            gStateMachine:change('game-over', {
                score = self.score,
                highScores = self.highScores
            })
        else
            if(self.paddle.size ~= 1) then
                self.paddle.size = self.paddle.size - 1
                self.paddle:changeDimensions()
            end
            gStateMachine:change('serve', {
                paddle = self.paddle,
                bricks = self.bricks,
                needKeyPowerUp = self.needKeyPowerUp,
                health = self.health,
                score = self.score,
                highScores = self.highScores,
                level = self.level,
                recoverPoints = self.recoverPoints
            })
        end
    end

    -- for rendering particle systems
    for k, brick in pairs(self.bricks) do
        brick:update(dt)
    end

    if love.keyboard.wasPressed('escape') then
        love.event.quit()
    end
end

function PlayState:render()
    -- render bricks
    for k, brick in pairs(self.bricks) do
        brick:render()
    end

    -- render all particle systems
    for k, brick in pairs(self.bricks) do
        brick:renderParticles()
    end

    self.paddle:render()

    if(self.ball1 ~= nil) then
        self.ball1:render()
    end
    if(self.ball2 ~= nil) then
        self.ball2:render()
    end
    if(self.ball3 ~= nil) then
        self.ball3:render()
    end


    self.powerUp:render()
    if(self.needKeyPowerUp) then
        self.keyPowerUp:render()

        if(self.hasKey) then
            love.graphics.draw(gTextures['powerups'], gFrames['powerups'][10], VIRTUAL_WIDTH - 16, VIRTUAL_HEIGHT - 16)
        end
    end


    renderScore(self.score)
    renderHealth(self.health)

    -- pause text, if paused
    if self.paused then
        love.graphics.setFont(gFonts['large'])
        love.graphics.printf("PAUSED", 0, VIRTUAL_HEIGHT / 2 - 16, VIRTUAL_WIDTH, 'center')
    end
end

function PlayState:checkVictory()
    for k, brick in pairs(self.bricks) do
        if brick.inPlay then
            return false
        end 
    end

    return true
end