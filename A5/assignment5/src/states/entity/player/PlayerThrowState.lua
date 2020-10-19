PlayerThrowState = Class{__includes = BaseState}

function PlayerThrowState:init(player, dungeon)
    self.player = player
    self.dungeon = dungeon

    -- render offset for spaced character sprite
    self.player.offsetY = 5
    self.player.offsetX = 0

    self.player:changeAnimation('pot-throw-' .. self.player.direction)
end

function PlayerThrowState:enter(params)
    -- call the throw method in game object
    self.player.heldObj:throw(self.player.direction, self.dungeon)

    self.player.currentAnimation:refresh()
    
    self.player.heldObj = nil
    
end

function PlayerThrowState:update(dt)
    -- after we've thrown the pot set state to idle
    if(self.player.currentAnimation.timesPlayed > 0) then
        self.player.currentAnimation.timesPlayed = 0
        self.player:changeState('idle')
    end
end

function PlayerThrowState:render()
    local anim = self.player.currentAnimation
    love.graphics.draw(gTextures[anim.texture], gFrames[anim.texture][anim:getCurrentFrame()],
        math.floor(self.player.x - self.player.offsetX), math.floor(self.player.y - self.player.offsetY))

end