# Tom Postler, 2017-11
# Powershell profile

function Get-WittyResponse {
    <#
        .SYNOPSIS

        Gets a very non-witty response.


        .DESCRIPTION

        Will always return 'Hello world!', but this was needed as an example.
    #>

    return "Hello world!"
}



# Is the current directory a git repository/working copy?
function isCurrentDirectoryGitRepository {
    if ((Test-Path ".git") -eq $true) {
        return $true
    }
    
    # Test within parent dirs
    $checkIn = (Get-Item .).parent
    while ($checkIn -ne $null) {
        $pathToTest = $checkIn.fullname + '/.git'
        if ((Test-Path $pathToTest) -eq $true) {
            return $true
        }
        else {
            $checkIn = $checkIn.parent
        }
    }
    
    return $false
}



# Extracts status details about the repo
function gitStatus {
    $untracked = $false
    $stadded = 0
    $stmodified = 0
    $stdeleted = 0
    $unstaged = $false
    $added = 0
    $modified = 0
    $deleted = 0
    $ahead = $false
    $aheadCount = 0
    $behind = $false
    $behindCount = 0
    $diverged = $false
    
    $output = git status
    
    $branchbits = $output[0].Split(' ')
    $branch = $branchbits[$branchbits.length - 1]
    
    $output | ForEach-Object {
        if ($_ -match "^.*ahead of 'origin/.*' by (\d+) commit.*") {
            $aheadCount = $matches[1]
            $ahead = $true
        }
        elseif ($_ -match "^.*behind 'origin/.*' by (\d+) commit.*") {
            $behindCount = $matches[1]
            $behind = $true
        }
        elseif ($_ -match "^.*'origin/.*' have diverged") {
            $diverged = $true
        }
        elseif ($_ -match "Changes not staged for commit") {
            $unstaged = $true
        }
        elseif ($_ -match "deleted:") {
            if ($unstaged) {
                $deleted += 1
            }
            else {
                $stdeleted += 1
            }
        }
        elseif (($_ -match "modified:") -or ($_ -match "renamed:")) {
            if ($unstaged) {
                $modified += 1
            }
            else {
                $stmodified += 1
            }
        }
        elseif ($_ -match "new file:") {
            if ($unstaged) {
                $added += 1
            }
            else {
                $stadded += 1
            }
        }
        elseif ($_ -match "Untracked files:") {
            $untracked = $true
        }
    }
    
    return @{
        "untracked"   = $untracked;
        "stadded"     = $stadded;
        "stmodified"  = $stmodified;
        "stdeleted"   = $stdeleted;
        "stall"       = $stadded + $stmodified + $stdeleted;
        "added"       = $added;
        "modified"    = $modified;
        "deleted"     = $deleted;
        "all"         = $added + $modified + $deleted;
        "ahead"       = $ahead;
        "aheadCount"  = $aheadCount;
        "behind"      = $behind;
        "behindCount" = $behindCount;
        "diverged"    = $diverged;
        "branch"      = $branch
    }
}



function Print-GitStatusText {
    <#
        .SYNOPSIS

        Gets some nice git status text when the current directory is within a git repository.


        .DESCRIPTION

        Basically copies the core functionality of posh-git in a way that makes it easier for me. Yay public licensing!

        Based off of https://markembling.info/2009/09/my-ideal-powershell-prompt-with-git-integration
    #>

    # If no git, do nothing
    if (((Get-Command "git.exe" -ErrorAction SilentlyContinue) -eq $null) -or (-not (isCurrentDirectoryGitRepository))) {
        return
    }

    # Get the current git branch
    $currentBranch = ''
    git branch | ForEach-Object {
        if ($_ -match "^\* (.*)") {
            $currentBranch += $matches[1]
        }
    }

    $status = gitStatus
    Write-Host('[') -NoNewline -ForegroundColor Yellow
    if ($status["ahead"]) {
        # We are ahead of origin
        Write-Host($currentBranch + [char]0x2191) -NoNewline -ForegroundColor Green
    }
    elseif ($status["behind"]) {
        # We are behind of origin
        Write-Host($currentBranch + [char]0x2193) -NoNewline -ForegroundColor Yellow
    }
    elseif ($status["diverged"]) {
        # We are diverged of origin
        Write-Host($currentBranch + [char]0x2195) -NoNewline -ForegroundColor Red
    }
    else {
        # We are equal origin, or unknown
        Write-Host($currentBranch + '') -NoNewline -ForegroundColor Cyan
    }

    if ($status["stadded"] -gt 0 -or $status["stmodified"] -gt 0 -or $status["stdeleted"] -gt 0) {
        Write-Host(' +' + $status["stadded"]) -NoNewline -ForegroundColor Green
        Write-Host(' ~' + $status["stmodified"]) -NoNewline -ForegroundColor Green
        Write-Host(' -' + $status["stdeleted"]) -NoNewline -ForegroundColor Green
    }

    if ($status["added"] -gt 0 -or $status["modified"] -gt 0 -or $status["deleted"] -gt 0) {
        if ($status["stall"] -gt 0) {
            Write-Host(' |') -NoNewline -ForegroundColor Yellow
        }
        Write-Host(' +' + $status["added"]) -NoNewline -ForegroundColor Red
        Write-Host(' ~' + $status["modified"]) -NoNewline -ForegroundColor Red
        Write-Host(' -' + $status["deleted"]) -NoNewline -ForegroundColor Red
    }
        
    if ($status["untracked"] -ne $false) {
        if ($status["stall"] -gt 0 -or $status["all"] -gt 0) {
            Write-Host(' |') -NoNewline -ForegroundColor Yellow
        }
        Write-Host(' !') -NoNewline -ForegroundColor Red
    }
        
    Write-Host(']') -NoNewline -ForegroundColor Yellow 
}



# Update the PS prompt to be nicer
Set-Variable -Name MaxPathLength -Value 100
function prompt {
    # Change home dir to ~
    $curPath = $ExecutionContext.SessionState.Path.CurrentLocation.Path
    if ($curPath.ToLower().StartsWith($HOME.ToLower())) {
        $curPath = "~" + $curPath.SubString($HOME.Length)
    }

    # Chop path
    if ($curPath.Length -gt $MaxPathLength) {
        $curPath = '...' + $curPath.SubString($curPath.Length - $MaxPathLength + 3)
    }

    Write-Host "PS $curPath " -NoNewline
    Print-GitStatusText
    return '> '
}



#
# Beyond this point are useful functions for PS
#



# Todo: add more functions
function Write-ColorSamples {
    <#
        .SYNOPSIS

        Prints a sample of the console colors available with Write-Host.


        .DESCRIPTION

        When given no information, walks through ConsoleColor and enumerates all possible color combinations.


        .PARAMETER ForegroundColor

        When provided, only enumerates through the one color.

        .PARAMETER BackgroundColor

        When provided, only enumerates through the one color.
    #>

    param
    (
        [ConsoleColor[]]
        $ForegroundColor,

        [ConsoleColor[]]
        $BackgroundColor
    )

    if (-not $ForegroundColor) {
        $ForegroundColor = [Enum]::GetValues([ConsoleColor])
    }
    if (-not $BackgroundColor) {
        $BackgroundColor = [Enum]::GetValues([ConsoleColor])
    }

    foreach ($bcolor in $BackgroundColor) {
        foreach ($fcolor in $ForegroundColor) {
            if ($bcolor -ne $fcolor) {
                Write-Host -ForegroundColor $fcolor -BackgroundColor $bcolor "BackgroundColor:$bcolor ForegroundColor:$fcolor"
            }
        }
    }
}



function Start-Sleep {
    [CmdletBinding(DefaultParameterSetName = 'Seconds', HelpUri = 'https://go.microsoft.com/fwlink/?LinkID=113407')]
    param(
        [Parameter(ParameterSetName = 'Seconds', Mandatory = $true, Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [ValidateRange(0, 2147483)]
        [int]
        ${Seconds},

        [Parameter(ParameterSetName = 'Milliseconds', Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [ValidateRange(0, 2147483647)]
        [int]
        ${Milliseconds},

        [Parameter(ParameterSetName = 'Minutes', Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [ValidateRange(0, 35791)]
        [float]
        ${Minutes}
    )

    begin {
        try {
            $outBuffer = $null
            if ($PSBoundParameters.TryGetValue('OutBuffer', [ref]$outBuffer)) {
                $PSBoundParameters['OutBuffer'] = 1
            }
            if ($PSBoundParameters['Minutes']) {
                $PSBoundParameters.Remove('Minutes') | Out-Null
                $PSBoundParameters['Milliseconds'] = [int]($Minutes * 60000)
                Write-Host "Sleeping until $([DateTime]::Now.AddMinutes($Minutes))"
            }
            $wrappedCmd = $ExecutionContext.InvokeCommand.GetCommand('Microsoft.PowerShell.Utility\Start-Sleep', [System.Management.Automation.CommandTypes]::Cmdlet)
            $scriptCmd = {& $wrappedCmd @PSBoundParameters }
            $steppablePipeline = $scriptCmd.GetSteppablePipeline($myInvocation.CommandOrigin)
            $steppablePipeline.Begin($PSCmdlet)
        }
        catch {
            throw
        }
    }

    process {
        try {
            $steppablePipeline.Process($_)
        }
        catch {
            throw
        }
    }

    end {
        try {
            $steppablePipeline.End()
        }
        catch {
            throw
        }
    }
    <#

.ForwardHelpTargetName Microsoft.PowerShell.Utility\Start-Sleep
.ForwardHelpCategory Cmdlet

#>
}