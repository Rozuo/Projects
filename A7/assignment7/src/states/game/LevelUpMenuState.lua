LevelUpMenuState = Class{__includes = BaseState}

function LevelUpMenuState:init(battleState, hpIncrease, attackIncrease, defenseIncrease, speedIncrease)
    self.battleState = battleState
    self.playerPokemon = self.battleState.player.party.pokemon[1]

    self.levelUpMenu = Menu{
        x = VIRTUAL_WIDTH/2,
        y = VIRTUAL_HEIGHT/2 - 96,
        width = 176,
        height = 160,
        items = {
            {
                text = 'HP: '.. tostring(self.playerPokemon.HP - hpIncrease) .. ' + ' .. tostring(hpIncrease) .. ' = ' .. tostring(self.playerPokemon.HP),
                onSelect = function()
                     -- fade in
                    gStateStack:push(FadeInState({
                        r = 255, g = 255, b = 255
                    }, 1, function() 
                        gSounds['victory-music']:stop()
                        -- resume field music
                        gSounds['field-music']:play()
                        
                        gStateStack:pop()
                        gStateStack:pop()
                        gStateStack:push(FadeOutState({
                            r = 255, g = 255, b = 255
                        }, 1, function() end))
                    end))
                end
            },
            {
                text = 'attack: '.. tostring(self.playerPokemon.attack - attackIncrease) .. ' + ' .. tostring(attackIncrease) .. ' = ' .. tostring(self.playerPokemon.attack),
                onSelect = function()
                     -- fade in
                     gStateStack:push(FadeInState({
                        r = 255, g = 255, b = 255
                    }, 1, function() 
                        gSounds['victory-music']:stop()
                        -- resume field music
                        gSounds['field-music']:play()
                        
                        gStateStack:pop()
                        gStateStack:pop()
                        gStateStack:push(FadeOutState({
                            r = 255, g = 255, b = 255
                        }, 1, function() end))
                    end))
                end
            },
            {
                text = 'defense: '.. tostring(self.playerPokemon.defense - defenseIncrease) .. ' + ' .. tostring(defenseIncrease) .. ' = ' .. tostring(self.playerPokemon.defense),
                onSelect = function()
                     -- fade in
                     gStateStack:push(FadeInState({
                        r = 255, g = 255, b = 255
                    }, 1, function() 
                        gSounds['victory-music']:stop()
                        -- resume field music
                        gSounds['field-music']:play()
                        
                        gStateStack:pop()
                        gStateStack:pop()
                        gStateStack:push(FadeOutState({
                            r = 255, g = 255, b = 255
                        }, 1, function() end))
                    end))
                end
            },
            {
                text = 'speed: '.. tostring(self.playerPokemon.speed - speedIncrease) .. ' + ' .. tostring(speedIncrease) .. ' = ' .. tostring(self.playerPokemon.speed),
                onSelect = function()
                     -- fade in
                     gStateStack:push(FadeInState({
                        r = 255, g = 255, b = 255
                    }, 1, function() 
                        gSounds['victory-music']:stop()
                        -- resume field music
                        gSounds['field-music']:play()
                        
                        gStateStack:pop()
                        gStateStack:pop()
                        gStateStack:push(FadeOutState({
                            r = 255, g = 255, b = 255
                        }, 1, function() end))
                    end))
                end
            }

        }
    }

    self.levelUpMenu.selection:setCursor(false)
end

function LevelUpMenuState:update(dt)
    self.levelUpMenu:update(dt)
end

function LevelUpMenuState:render()
    self.levelUpMenu:render()
end