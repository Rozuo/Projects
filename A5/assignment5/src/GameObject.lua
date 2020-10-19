--[[
    GD50
    Legend of Zelda

    Author: Colton Ogden
    cogden@cs50.harvard.edu
]]

GameObject = Class{}

function GameObject:init(def, x, y)
    -- string identifying this object type
    self.type = def.type

    self.texture = def.texture
    self.frame = def.frame or 1

    -- whether it acts as an obstacle or not
    self.solid = def.solid
    self.consumable = def.consumable
    -- self.collidable = def.collidable

    self.defaultState = def.defaultState
    self.state = self.defaultState
    self.states = def.states

    -- dimensions
    self.x = x
    self.y = y
    self.width = def.width
    self.height = def.height


    -- speed of the object when thrown we preset the values if nothing was inputted
    self.dx = def.dx or 80
    self.dy = def.dy or 80

    -- default empty collision callback
    self.onCollide = def.onCollide or function() end

    -- self.onConsume = def.onConsume

    self.hitBox = Hitbox(self.x, self.y, self.width, self.height)

    -- 16 (tile size) * 4 = 64
    self.maxThrownDistanceX = def.maxThrownDistanceX or 64
    self.maxThrownDistanceY = def.maxThrownDistanceY or 64

    self.dungeon = nil
    self.thrown = false
    self.direction = nil
    self.destroyed = def.destroyed or false
end

function GameObject:update(dt)
    self.hitBox = Hitbox(self.x, self.y, self.width, self.height)
    if(self.thrown) then
        if(self.direction == 'left') then
            self.x = self.x - self.dx * dt
            
            for k, entity in pairs(self.dungeon.currentRoom.entities) do
                if(not entity.dead and entity:collides(self.hitBox)) then
                    entity:damage(1)
                    gSounds['hit-enemy']:play()
                    self.destroyed = true
                    self.thrown = false
                end
            end

            if self.x <= MAP_RENDER_OFFSET_X + TILE_SIZE then 
                self.x = MAP_RENDER_OFFSET_X + TILE_SIZE
                self.destroyed = true
                self.thrown = false
            elseif self.x <= self.maxThrownDistanceX then
                self.destroyed = true
                self.thrown = false
            end
        elseif self.direction == 'right' then
            self.x = self.x + self.dx * dt

            for k, entity in pairs(self.dungeon.currentRoom.entities) do
                if(not entity.dead and entity:collides(self.hitBox)) then
                    entity:damage(1)
                    gSounds['hit-enemy']:play()
                    self.destroyed = true
                    self.thrown = false
                end
            end

            if self.x + self.width >= VIRTUAL_WIDTH - TILE_SIZE * 2 then
                self.x = VIRTUAL_WIDTH - TILE_SIZE * 2 - self.width
                self.destroyed = true
                self.thrown = false
            elseif self.x >= self.maxThrownDistanceX then
                self.destroyed = true
                self.thrown = false
            end
        elseif self.direction == 'up' then
            self.y = self.y - self.dy * dt

            for k, entity in pairs(self.dungeon.currentRoom.entities) do
                if(not entity.dead and entity:collides(self.hitBox)) then
                    entity:damage(1)
                    gSounds['hit-enemy']:play()
                    self.destroyed = true
                    self.thrown = false
                end
            end

            if self.y <= MAP_RENDER_OFFSET_Y + TILE_SIZE - self.height / 2 then 
                self.y = MAP_RENDER_OFFSET_Y + TILE_SIZE - self.height / 2
                self.destroyed = true
                self.thrown = false
            elseif self.y <= self.maxThrownDistanceY then
                self.destroyed = true
                self.thrown = false
            end
        elseif self.direction == 'down' then
            self.y = self.y + self.dy * dt

            for k, entity in pairs(self.dungeon.currentRoom.entities) do
                if(not entity.dead and entity:collides(self.hitBox)) then
                    entity:damage(1)
                    gSounds['hit-enemy']:play()
                    self.destroyed = true
                    self.thrown = false
                end
            end

            local bottomEdge = VIRTUAL_HEIGHT - (VIRTUAL_HEIGHT - MAP_HEIGHT * TILE_SIZE) 
            + MAP_RENDER_OFFSET_Y - TILE_SIZE

            if self.y + self.height >= bottomEdge then
                self.y = bottomEdge - self.height
                self.destroyed = true
                self.thrown = false
            elseif self.y >= self.maxThrownDistanceY then
                self.destroyed = true
                self.thrown = false
            end
        end
    end
end

function GameObject:render(adjacentOffsetX, adjacentOffsetY)
    love.graphics.draw(gTextures[self.texture], gFrames[self.texture][self.states[self.state].frame or self.frame],
        self.x + adjacentOffsetX, self.y + adjacentOffsetY)

    -- love.graphics.setColor(255, 0, 255, 255)
    -- love.graphics.rectangle('line', self.hitBox.x, self.hitBox.y,
    --     self.hitBox.width, self.hitBox.height)
    -- love.graphics.setColor(255, 255, 255, 255)
end

function GameObject:throw(direction, dungeon)
    self.thrown = true
    self.direction = direction
    
    if(self.direction == 'left') then
        self.maxThrownDistanceX =  self.x - self.maxThrownDistanceX
    else
        self.maxThrownDistanceX = self.maxThrownDistanceX + self.x
    end

    if(self.direction == 'up') then
        self.maxThrownDistanceY = self.y - self.maxThrownDistanceY
    else
        self.maxThrownDistanceY = self.maxThrownDistanceY + self.y
    end
    
    self.dungeon = dungeon
end

function GameObject:collides(target) 
    return not (self.x + self.width < target.x or self.x > target.x + target.width or
                self.y + self.height < target.y or self.y > target.y + target.height)
end