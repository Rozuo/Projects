PauseState = Class{__include = BaseState}

-- pause icon made by Pixel perfect from www.flaticon.com
-- https://www.flaticon.com/free-icon/pause_633940?term=pause&page=1&position=2

function PauseState:init()
    self.pauseIcon = love.graphics.newImage("pause-icon.png")
    -- self.score = 0
    -- self.pipePairs = {}
    -- self.timer = 0
    -- self.bird = nil
end

function PauseState:enter(params)
    sounds['music']:stop()
    self.score = params.score
    self.pipePairs = params.pipePairs
    self.timer = params.timer
    self.bird = params.bird
end

function PauseState:update(dt)
    if(love.keyboard.wasPressed('p')) then
        sounds['pause']:play()
        gStateMachine:change("play", {score = self.score, pipePairs = self.pipePairs, timer = self.timer, bird = self.bird})
    end
end

function PauseState:exit()
    self.pauseIcon = false
    sounds['music']:play()
end

function PauseState:render()
    if(self.pipePairs ~= nil) then
        for k, pair in pairs(self.pipePairs) do
            pair:render()
        end
    end

    love.graphics.setFont(flappyFont)
    love.graphics.print('Score: ' .. tostring(self.score), 8, 8)

    self.bird:render()
    love.graphics.draw(self.pauseIcon, VIRTUAL_WIDTH/2-32, VIRTUAL_HEIGHT/2-32)
end
