
PlayerPickUpState = Class{__includes = BaseState}

function PlayerPickUpState:init(player, dungeon)
    self.player = player
    self.dungeon = dungeon

    -- render offset for spaced character sprite
    self.player.offsetY = 5
    self.player.offsetX = 0

    -- create hitbox based on where the player is and facing
    local direction = self.player.direction
    
    local hitboxX, hitboxY, hitboxWidth, hitboxHeight

    if direction == 'left' then
        hitboxWidth = 8
        hitboxHeight = 16
        hitboxX = self.player.x - hitboxWidth
        hitboxY = self.player.y + 2
    elseif direction == 'right' then
        hitboxWidth = 8
        hitboxHeight = 16
        hitboxX = self.player.x + self.player.width
        hitboxY = self.player.y + 2
    elseif direction == 'up' then
        hitboxWidth = 16
        hitboxHeight = 8
        hitboxX = self.player.x
        hitboxY = self.player.y - hitboxHeight
    else
        hitboxWidth = 16
        hitboxHeight = 8
        hitboxX = self.player.x
        hitboxY = self.player.y + self.player.height
    end

    self.pickUpHitBox = Hitbox(hitboxX, hitboxY, hitboxWidth, hitboxHeight)
    
    self.pickedUpObj = nil

    self.player:changeAnimation('pot-lift-' .. self.player.direction)
end


function PlayerPickUpState:enter(params)
    for k, object in pairs(self.dungeon.currentRoom.objects) do
        if object.solid and object:collides(self.pickUpHitBox) then
            
            self.player.heldObj = object
            self.player.heldObj.solid = false
            Timer.tween(0.1, {
                [self.player.heldObj] = {
                    x = self.player.x,
                    y = self.player.y - self.player.height/2 - 2
                }
            })
            self.player.currentAnimation:refresh()
        end
    end
    
    if(self.player.heldObj == nil) then
        self.player:changeState('idle')
    end

end


function PlayerPickUpState:update(dt)
    if(self.player.heldObj ~= nil and self.player.currentAnimation.timesPlayed > 0) then
        self.player.currentAnimation.timesPlayed = 0
        self.player:changeState('pick-up-idle')
    end
end

function PlayerPickUpState:render()
    local anim = self.player.currentAnimation
    love.graphics.draw(gTextures[anim.texture], gFrames[anim.texture][anim:getCurrentFrame()],
        math.floor(self.player.x - self.player.offsetX), math.floor(self.player.y - self.player.offsetY))

    -- debug for player and hurtbox collision rects
    -- love.graphics.setColor(255, 0, 255, 255)
    -- love.graphics.rectangle('line', self.player.x, self.player.y, self.player.width, self.player.height)
    -- love.graphics.rectangle('line', self.pickUpHitBox.x, self.pickUpHitBox.y,
    --     self.pickUpHitBox.width, self.pickUpHitBox.height)
    -- love.graphics.setColor(255, 255, 255, 255)
end