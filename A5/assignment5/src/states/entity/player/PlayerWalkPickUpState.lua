PlayerWalkPickUpState = Class{__includes = EntityWalkState}

function PlayerWalkPickUpState:init(player, dungeon)
    self.entity = player
    self.dungeon = dungeon

    -- render offset for spaced character sprite
    self.entity.offsetY = 5
    self.entity.offsetX = 0

    self.pickedUpObj = nil
end

function PlayerWalkPickUpState:enter(params)
    -- readjust the pots location
    if(self.entity.heldObj ~= nil) then
        self.entity.heldObj.x = self.entity.x
        self.entity.heldObj.y = self.entity.y - self.entity.height/2 - 2
    end
end

function PlayerWalkPickUpState:update(dt)
    if love.keyboard.isDown('left') then
        self.entity.direction = 'left'
        self.entity:changeAnimation('pot-walk-left')
    elseif love.keyboard.isDown('right') then
        self.entity.direction = 'right'
        self.entity:changeAnimation('pot-walk-right')
    elseif love.keyboard.isDown('up') then
        self.entity.direction = 'up'
        self.entity:changeAnimation('pot-walk-up')
    elseif love.keyboard.isDown('down') then
        self.entity.direction = 'down'
        self.entity:changeAnimation('pot-walk-down')
    else
        if(self.entity.heldObj ~= nil) then
            self.entity.heldObj.x = self.entity.x
            self.entity.heldObj.y = self.entity.y - self.entity.height/2 - 2
        end
        self.entity:changeState('pick-up-idle')
    end

    if(self.entity.heldObj ~= nil) then
        self.entity.heldObj.x = self.entity.x
        self.entity.heldObj.y = self.entity.y - self.entity.height/2 - 2
    end

    -- perform base collision detection against walls
    EntityWalkState.update(self, dt)

    -- if we bumped something when checking collision, check any object collisions
    if self.bumped then
        if self.entity.direction == 'left' then
            
            -- temporarily adjust position
            self.entity.x = self.entity.x - PLAYER_WALK_SPEED * dt

            -- readjust
            self.entity.x = self.entity.x + PLAYER_WALK_SPEED * dt
        elseif self.entity.direction == 'right' then
            
            -- temporarily adjust position
            self.entity.x = self.entity.x + PLAYER_WALK_SPEED * dt

            -- readjust
            self.entity.x = self.entity.x - PLAYER_WALK_SPEED * dt
        elseif self.entity.direction == 'up' then
            
            -- temporarily adjust position
            self.entity.y = self.entity.y - PLAYER_WALK_SPEED * dt

            -- readjust
            self.entity.y = self.entity.y + PLAYER_WALK_SPEED * dt
        else
            
            -- temporarily adjust position
            self.entity.y = self.entity.y + PLAYER_WALK_SPEED * dt

            -- readjust
            self.entity.y = self.entity.y - PLAYER_WALK_SPEED * dt
        end
    end


    if(love.keyboard.wasPressed('return')) then
        self.entity:changeState('throw-obj')
    end
end