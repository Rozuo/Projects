

PlayerIdlePickUpState = Class{__includes = BaseState}

function PlayerIdlePickUpState:init(player)
    self.player = player

    -- render offset for spaced character sprite
    self.player.offsetY = 5
    self.player.offsetX = 0

    self.player:changeAnimation('pot-idle-' .. self.player.direction)
end

function PlayerIdlePickUpState:enter(params)

end

function PlayerIdlePickUpState:update(dt)
    if love.keyboard.isDown('left') or love.keyboard.isDown('right') or
       love.keyboard.isDown('up') or love.keyboard.isDown('down') then
        self.player:changeState('pick-up-walk', {
            pickedUpObj = self.pickedUpObj})
    end

    if love.keyboard.wasPressed('return') then
        self.player:changeState('throw-obj')
    end
end

function PlayerIdlePickUpState:render()
    local anim = self.player.currentAnimation
    love.graphics.draw(gTextures[anim.texture], gFrames[anim.texture][anim:getCurrentFrame()],
        math.floor(self.player.x - self.player.offsetX), math.floor(self.player.y - self.player.offsetY))

end