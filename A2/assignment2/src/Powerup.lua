
Powerup = Class{}


function Powerup:init()
    
    self.y = VIRTUAL_HEIGHT/2 - 32

    self.dy = 14

    self.width = 16
    self.height = 16

    self.x = math.random(self.width, VIRTUAL_WIDTH - self.width)

    self.texture = gFrames['powerups'][9]

    self.inPlay = false
end

function Powerup:update(dt)
    self.y = self.y + self.dy *dt
end

function Powerup:collides(target)
    --collision code taken from ball.lua
    if self.x > target.x + target.width or target.x > self.x + self.width then
        return false
    end

    if self.y > target.y + target.height or target.y > self.y + self.height then
        return false
    end 

    return true
end


function Powerup:render()
    if self.inPlay then
        love.graphics.draw(gTextures['powerups'], self.texture, self.x, self.y)
    end
end