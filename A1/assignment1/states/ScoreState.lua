--[[
    ScoreState Class
    Author: Colton Ogden
    cogden@cs50.harvard.edu

    A simple state used to display the player's score before they
    transition back into the play state. Transitioned to from the
    PlayState when they collide with a Pipe.
]]

ScoreState = Class{__includes = BaseState}

-- bronze medal icon made by Freepik  from www.flaticon.com
-- https://www.flaticon.com/free-icon/medal_2583434?term=medal&page=1&position=23
local bronzeMedal = love.graphics.newImage("bronze-medal.png")

-- silver medal icon made by Freepik  from www.flaticon.com
-- https://www.flaticon.com/free-icon/medal_2583319?term=medal&page=1&position=25
local silverMedal = love.graphics.newImage("silver-medal.png")

-- gold medal icon made by Vectors Market from www.flaticon.com
-- https://www.flaticon.com/free-icon/gold-medal_744984?term=medal&page=1&position=11
local goldMedal   = love.graphics.newImage("gold-medal.png")


--[[
    When we enter the score state, we expect to receive the score
    from the play state so we know what to render to the State.
]]
function ScoreState:enter(params)
    self.score = params.score
end

function ScoreState:update(dt)
    -- go back to play if enter is pressed
    if love.keyboard.wasPressed('enter') or love.keyboard.wasPressed('return') then
        gStateMachine:change('countdown')
    end
end

function ScoreState:render()
    -- simply render the score to the middle of the screen
    love.graphics.setFont(flappyFont)
    love.graphics.printf('Oof! You lost!', 0, 64, VIRTUAL_WIDTH, 'center')

    love.graphics.setFont(mediumFont)
    love.graphics.printf('Score: ' .. tostring(self.score), 0, 100, VIRTUAL_WIDTH, 'center')

    love.graphics.printf('Press Enter to Play Again!', 0, 160, VIRTUAL_WIDTH, 'center')

    -- draw a medal depending on the score.
    -- we substract 16 from the half of the virtual width because it centers the image.
    -- and 115 for the y was chosen because of the placement of the texts. As I wanted the 
    -- images to be between play again and the score display.
    if self.score >= 4 then
        love.graphics.draw(goldMedal, VIRTUAL_WIDTH/2-16, 115)
    elseif self.score >= 3 then
        love.graphics.draw(silverMedal, VIRTUAL_WIDTH/2-16, 115)
    elseif self.score >= 2 then
        love.graphics.draw(bronzeMedal, VIRTUAL_WIDTH/2-16, 115)
    end
end