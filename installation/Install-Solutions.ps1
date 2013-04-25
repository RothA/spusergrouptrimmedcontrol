function Install-Solutions([string]$configFile)
{
    if ([string]::IsNullOrEmpty($configFile)) { return }

    [xml]$solutionsConfig = Get-Content $configFile
    if ($solutionsConfig -eq $null) { return }

    $solutionsConfig.Solutions.Solution | ForEach-Object {
        [string]$path = $_.Path
        [bool]$gac = [bool]::Parse($_.GACDeployment)
        [bool]$cas = [bool]::Parse($_.CASPolicies)
        $webApps = $_.WebApplications.WebApplication
        Install-Solution $path $gac $cas $webApps
    }
}

function Install-Solution([string]$path, [bool]$gac, [bool]$cas, [string[]]$webApps = @())
{
    $spAdminServiceName = "SPAdminV4"

    [string]$name = Split-Path -Path $path -Leaf
    $solution = Get-SPSolution $name -ErrorAction SilentlyContinue
    
    if ($solution -ne $null) {
        #Retract the solution
        if ($solution.Deployed) {
            Write-Host "Retracting solution $name..."
            if ($solution.ContainsWebApplicationResource) {
                $solution | Uninstall-SPSolution -AllWebApplications -Confirm:$false
            } else {
                $solution | Uninstall-SPSolution -Confirm:$false
            }
            Stop-Service -Name $spAdminServiceName
            Start-SPAdminJob -Verbose
            Start-Service -Name $spAdminServiceName    
        
            #Block until we're sure the solution is no longer deployed.
            do { Start-Sleep 2 } while ((Get-SPSolution $name).Deployed)
        }
        
        #Delete the solution
        Write-Host "Removing solution $name..."
        Get-SPSolution $name | Remove-SPSolution -Confirm:$false
    }
    
    #Add the solution
    Write-Host "Adding solution $name..."
    $solution = Add-SPSolution $path
    
    #Deploy the solution
    if (!$solution.ContainsWebApplicationResource) {
        Write-Host "Deploying solution $name to the Farm..."
        $solution | Install-SPSolution -GACDeployment:$gac -CASPolicies:$cas -Confirm:$false -Force
    } else {
        if ($webApps -eq $null -or $webApps.Length -eq 0) {
            Write-Warning "The solution $name contains web application resources but no web applications were specified to deploy to."
            return
        }
        $webApps | ForEach-Object {
            Write-Host "Deploying solution $name to $_..."
            $solution | Install-SPSolution -GACDeployment:$gac -CASPolicies:$cas -WebApplication $_ -Confirm:$false -Force
        }
    }
    Stop-Service -Name $spAdminServiceName
    Start-SPAdminJob -Verbose
    Start-Service -Name $spAdminServiceName    
    
    #Block until we're sure the solution is deployed.
    do { Start-Sleep 2 } while (!((Get-SPSolution $name).Deployed))
}

Install-Solutions "solutions.xml"