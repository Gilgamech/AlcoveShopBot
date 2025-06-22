#Copyright 2025 Gilgamech Technologies
#Author: Stephen Gillie
#Created 04/20/2025
#Updated 06/12/2025
#Alcove was the new finditem.


$build = 16
$FullText = "is now full"#[05:33:18] [Render thread/INFO]: [System] [CHAT] SHOPS ▶ Your shop at 15274, 66, 20463 is now full.
$SellText = "to your shop" #[05:40:12] [Render thread/INFO]: [System] [CHAT] SHOPS ▶ kota490 sold 1728 Sea Lantern to your shop {3}.
$BuyText = "from your shop" #[05:47:12] [Render thread/INFO]: [System] [CHAT] SHOPS ▶ _Blackjack29313 purchased 2 Grindstone from your shop and you earned $9.50 ($0.50 in taxes).
$EmptyText = "has run out of"#[06:07:40] [Rend
$OldDate = get-date -f dd

$DataFile = "C:\Users\StephenGillie\Documents\PowerShell\AlcoveShopBot.csv"
$OwnerList = "C:\Users\StephenGillie\Documents\PowerShell\ChillPWOwnerList.csv"

 $Values = @{}
 $Values.Empty = "Empty"
 $Values.Full = "Full"
 $Values.Sell = "Sell"
 $Values.Buy = "Buy"
 $Values.Neg = "-"


function Get-MCLatestSales {
	param(
		$LogFile = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\latest.log"
	)
	$header2 = "Data","Message"
	$a = gc $LogFile;
	$b = $a -replace ",","" -replace "\]: ","," -replace "\[System\] \[CHAT\] ",""  -replace "\| ","" | ConvertFrom-Csv -Header $header2;
	$c = ($b | select-string "Successfully sold:").LineNumber;
	$e = @() | Select-Object "ItemName","Qty","Revenue","Taxes";
	
	$f = @();
	
	$c | %{
	$e = "" | Select-Object "ItemName","Qty","Revenue","Taxes";
	
	$e.Qty = ($b.message[$_] -split " ")[0]
	$e.ItemName = (($b.message[$_]) -replace "$($e.Qty) ","" -split " for")[0]
	$e.Revenue = ($b.message[$_] -split " ")[-1] -replace "\$",""
	$e.taxes = $b.message[$_+1] -replace "You paid \$","" -replace " in taxes.",""
	$f += $e
	};
	$qty = ($f.qty | Measure-Object -sum).sum;
	$rev = [math]::round(($f.Revenue | Measure-Object -sum).sum,2);
	$tax = ($f.Taxes | Measure-Object -sum).sum;
	"Today, I've sold $qty blocks for `$$rev in revenue (average `$$([math]::round($rev/$qty,2)) per block) and paid `$$tax in taxes ($([math]::round($tax/$rev,2))% tax rate)."
}

function Get-MCSalesData {
	param(
		$LogFile = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\latest.log"
	)
	$Year = (($LogFile -split "\\")[-1] -split "-")[0]
	$Month = (($LogFile -split "\\")[-1] -split "-")[1]
	$Day = (($LogFile -split "\\")[-1] -split "-")[2]
	$name = "$Year-$Month-$Day"

	$Header = "Info","Message"
	$a = gc $LogFile;
	$b = $a -replace ",","" -replace "\]: ","," -replace "\[System\] \[CHAT\] ",""  -replace "\| ","" | ConvertFrom-Csv -Header $Header;
	$c = ($b | select-string "Successfully sold:").LineNumber;
	$e = @() | Select-Object "ItemName","Qty","Revenue","Taxes";
	
	$f = @();
	
	$c | %{
		$e = "" | Select-Object "Date","Time","ItemName","ShopOwner","Qty","Revenue","Taxes";
		
		$ShopOwner = (($b.message[$_ -8]) -split "Owner: ")[1]
		if (!$ShopOwner) {$ShopOwner = (($b.message[$_ -9]) -split "Owner: ")[1]}
		if (!$ShopOwner) {$ShopOwner = (($b.message[$_ -10]) -split "Owner: ")[1]}

		$e.Date = $name
		$e.Time = ($b.Info[$_] -split "\] \[")[0] -replace "\[",""
		$e.Qty = ($b.message[$_] -split " ")[0]
		$e.ItemName = (($b.message[$_]) -replace "$($e.Qty) ","" -split " for")[0]
		$e.ShopOwner = $ShopOwner
		$e.Revenue = ($b.message[$_] -split " ")[-1] -replace "\$",""
		$e.taxes = $b.message[$_+1] -replace "You paid \$","" -replace " in taxes.",""
		$f += $e
	};
	$f | where {$_.ItemName}
}
#Build CSV: (ls -filter *.log).FullName | %{Get-MCSalesData $_ | ConvertTo-Csv -NoTypeInformation | Out-File -append data.csv} #Then manually remove headers

function Get-MCShopData {
	Param(
		$LogFile = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\latest.log",
		$Data = (((gc $LogFile) -split "`n" | select-string "SHOPS ▶ " | Where {$_ -notmatch "Enter all in chat"}) -replace "\[Render thread/INFO\]: \[System\] \[CHAT\] SHOPS ▶ ","")
	)
	$EmptyText = "*has run out of*"#[06:07:40] Your shop at 15226, 63, 20487 has run out of Cocoa Beans!
	#[07:12:51] [Render thread/INFO]: [System] [CHAT] SHOPS ▶ This shop has run out of space! Contact the shop owner or staff to get it emptied.
	$FullText = "*is now full*"#[05:33:18] Your shop at 15274, 66, 20463 is now full.
	$SellText = "*to your shop*" #[05:40:12] kota490 sold 1728 Sea Lantern to your shop {3}.
	$BuyText = "*from your shop*" #[05:47:12] _Blackjack29313 purchased 2 Grindstone from your shop and you earned $9.50 ($0.50 in taxes).
	$NameData = gc $dataFile | convertfrom-csv
	$out = @()
	$n = 0
	$Data = $Data | where {$_ -notmatch "out of space"}
	$Data = $Data | where {$_ -notmatch "how many you wish"}
	foreach ($Item in $Data) {
		$n++
		$new = "" | select ItemName,XLoc,YLoc,ZLoc,StockQty,Event,DateTime,PlayerName
		#Use all 3 coords because some places have 2 buy shops stacked at the same X and Z. 
		switch -wildcard ($Item) {
			$EmptyText {
				$SplitItem = $Item -replace "\[","" -replace "\] Your shop at","," -replace " has run out of","," -replace "!","" -split ", "
				#06:07:40 ,15226, 63, 20487, Cocoa Beans
				$new.DateTime = $SplitItem[0]
				$new.XLoc = $SplitItem[1]
				$new.YLoc = $SplitItem[2]
				$new.ZLoc = $SplitItem[3]
				$new.ItemName = $SplitItem[4]
				$new.Event = "Empty"
				$new.StockQty = ($data[$data.IndexOf($item)-1] -split " ")[3]
				$new.PlayerName = ($data[$data.IndexOf($item)-1] -split " ")[1]
			}
			$FullText {
				$SplitItem = $Item -replace "\[","" -replace "\] Your shop at","," -replace " is now full.",", " -replace "!","" -split ", "
				#05:33:18, 15274, 66, 20463,
				$new.DateTime = $SplitItem[0]
				$new.XLoc = $SplitItem[1]
				$new.YLoc = $SplitItem[2]
				$new.ZLoc = $SplitItem[3]
				#$new.ItemName = $SplitItem[4]
				$new.Event = "Full"
				$new.StockQty = ($data[$data.IndexOf($item)-1] -split " ")[3]
				$new.PlayerName = ($data[$data.IndexOf($item)-1] -split " ")[1]
			}
			$SellText {
			try {
				$SplitItem = $Item -replace "\[","" -replace "\]","," -replace " sold ",", " -replace " to your shop \{3\}.",", " -replace "!","" -split ", "
				 #05:40:12, kota490, 1728 Sea Lantern,
				$new.DateTime = $SplitItem[0]
				$new.PlayerName = $SplitItem[1]
				#$new.XLoc = $SplitItem[1]
				#$new.YLoc = $SplitItem[2]
				#$new.ZLoc = $SplitItem[3]
				$new.ItemName = ($SplitItem[2] -split " ")[1..9] -join " "
				$new.StockQty = [int]($SplitItem[2] -split " ")[0]
				$new.Event = "Sell"
				$new.PlayerName = $SplitItem[1]
				} catch {}
			}
			$BuyText {
				$SplitItem = $Item -replace "\[","" -replace "\]","," -replace " purchased ",", " -replace " from your shop and you earned ",", " -replace "\(","" -split ", "
				#05:47:12 _Blackjack29313, 2 Grindstone, $9.50 $0.50 in taxes).
				$new.DateTime = $SplitItem[0]
				$new.PlayerName = $SplitItem[1]
				#$new.XLoc = $SplitItem[1]
				#$new.YLoc = $SplitItem[2]
				#$new.ZLoc = $SplitItem[3]
				$new.ItemName = ($SplitItem[2] -split " ")[1..9] -join " "
				$new.StockQty = [int](-($SplitItem[2] -split " ")[0])
				$new.Event = "Buy"
				$new.PlayerName = $SplitItem[1]
			}
		}#end switch
		$pct = [Math]::round(($n/$data.length)*100)
		Write-Progress -Activity "Reading latest.log" -Status "Item $n of $($data.length) ($($pct)% complete)- $item" -PercentComplete $pct
		
		$NewName = $NameData | where {$_.Xloc -eq $new.Xloc} | where {$_.Yloc -eq $new.Yloc} | where {$_.Zloc -eq $new.Zloc}
		if ($NewName) {
			$new.ItemName = $NewName.ItemName
		}
		
		$out += $new
	}#end foreach
	Write-Progress -Activity "Reading latest.log" Complete



	$out
}#end function

#$a = (Get-MCShopData | where {$_.event -match "Empty"})[0]
#"Empty shop: $($a.PlayerName) has purchased the last $($a.StockQty) $($a.ItemName)!"

	#$getUrl ="https://discordapp.com/api/channels/1260720389495980053/messages?after=691723071383666748"

Function Get-DiscordMessage {
	$Headers = @{Authorization = "Bot $DiscordToken)"}
	
	$ChannelUrl ="https://discordapp.com/api/channels/1260720389495980053"
	$response = Invoke-RestMethod -ContentType "Application/JSON" -Uri $ChannelUrl -Method "GET" -Headers $Headers -UseBasicParsing

	$getUrl ="$ChannelUrl/messages/"+$response.last_message_id

	$response = Invoke-RestMethod -ContentType "Application/JSON" -Uri $GetURL -Method "GET" -Headers $Headers -UseBasicParsing
	$response.content
}

Function Send-DiscordMessage () {
	Param(
		$content,
		[switch]$WhatIf
	)
	$payload = [PSCustomObject]@{content = $content}
	$headers = @{"Content-Type"="application/json"}
	if ($WhatIf) {
		Write-Host "WhatIf: Post payload `"$($Payload.content)`" to Webhook $Webhook with headers $headers"
	} else {
		$null = Invoke-RestMethod -Uri $Webhook -Method Post -Body ($payload | ConvertTo-Json) -Headers $headers
	}
}

Function Run-Bot {
	$LogFile = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\latest.log"
	Write-Host "Alcove Shop Bot build $build online."
	$OldData = ""
	while ($true) {
		$ShopData = (Get-MCShopData | where {$_.event -match "Empty"});
		$content = try{(diff $OldData $ShopData).inputobject}catch{}
		#$content = (diff $ShopData $OldData -ErrorAction SilentlyContinue).inputobject;
		foreach ($item in $content){
		#if ($content){
			$out = "Empty shop: Player ``$($item.PlayerName)`` has purchased the last $($item.StockQty) $($item.ItemName)!";
			$out
			Send-DiscordMessage -content $out #-WhatIf
		};
		$OldData = $ShopData

		<#
		$CurrentDate = get-date -f dd
		if ($CurrentDate -ne $OldDate)  {
			Write-Host "Generating daily report."
			$Report = (Get-BuildDailyReport) -join "`n"
			if ($Report) {
				Send-DiscordMessage -content $Report
			$OldDate = $CurrentDate
			};
			While ($time -ne 0) {
				[int]$time = (get-date -f ss)
				#write-Host $time
				Sleep 1
			}
		}
		#>
		sleep 10
	}
}

Function Get-BuildDailyReport {
	Param(
		$day = (Get-Date (Get-Date).AddDays(-1) -f dd),
		$date = (get-date -Day $day -f "yyyy-MM-dd"),
		[Switch]$Weekly
	)
	$Data = Get-DailyData -day $day -date $date -Silent
	if ($Weekly) {
		Get-DailySales -Data $Data -Date (Get-Date (get-date $date) -Format "MMMM d") -Weekly
	} else {
		Get-DailySales -Data $Data -Date (Get-Date (get-date $date) -Format "MMMM d")
	}
}

Function Get-DailyData {
	Param(
		$day = (Get-Date (Get-Date).AddDays(-1) -f dd),
		$date = (get-date -Day $day -f "yyyy-MM-dd"),
		$logfolder = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\",
		[Switch]$Silent
	)
	$UnzipFiles = ls $logfolder | where {$_.fullname -match "$date"} | where {$_.fullname -match "gz"}
	$UnzipFiles | %{
		if (!$Silent) {
			Write-Host "Unzipping $_"
		}
			DeGZip-File -infile $_
	}
	$inputfiles = ls $logfolder | where {$_.fullname -match "$date"} | where {$_.fullname -notmatch "gz"}
	
	$Data = $null
	$inputfiles | %{
		if (!$Silent) {
			Write-Host "Reading $_"
		}
		$Data += (Get-MCShopData -LogFile $_)
	}
	return $Data
}

Function Get-BuildWeeklyReport {
	Param(
		$days = 14,
		$logfolder = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\"
	)
	
	"Date | Sold | Purchased | Turnover | OOS | Revenue"
	($days +1)..1 | %{
		$date = (get-date (get-date).AddDays(-$_) -f "yyyy-MM-dd")
		
		$Report = Get-BuildDailyReport -date $date -Silent -Weekly;
		$MoneyMade = Get-MoneyMade -date $date -NoFormat
		$Report + " | $MoneyMade"
	}
	

}

Function Get-DailySales {
	Param(
		$LogFile = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\latest.log",
		$Data = (Get-MCShopData -LogFile $LogFile | where {$_.event}),
		$Date = (Get-Date (get-date).AddDays(-1) -Format "MMMM d"),
		[switch]$Weekly
	)

	$Sold = 0
	$Data | where {$_.event -match $Values.Buy}  | %{
		try {
			$Sold += ($_.stockqty -replace "-","")
		} catch {}
	}
	
	$Purchased = 0
	$Data | where {$_.event -match $Values.Sell}  | %{
		try {
			$Purchased += ($_.stockqty -replace "-","")
		} catch {}
	}
	
	If ($Purchased -gt $Sold) {
		$TurnoverAmt = $Purchased - $Sold
		$TurnoverAmt = '{0:N0}' -f $TurnoverAmt
		$Sold = '{0:N0}' -f $Sold
		$Purchased = '{0:N0}' -f $Purchased
		$Turnover = "$TurnoverAmt more purchased than sold." 
	} elseif ($Purchased -lt $Sold) {
		$TurnoverAmt = $Sold - $Purchased
		$TurnoverAmt = '{0:N0}' -f $TurnoverAmt
		$Sold = '{0:N0}' -f $Sold
		$Purchased = '{0:N0}' -f $Purchased
		$Turnover = "$TurnoverAmt more sold than purchased." 
	} else {
		$Turnover = "Exact same amount purchased as sold. (This is rare, please double-check.)" 
	}
	$OOS = ($data | where {$_.Event -match "Empty"}).count
	#$OOS = ($data.StockQty | where {$_EVent -match $Values.Empty}).count
	
$out = ""
if ($Weekly) {
$out = "$($Date) | $Sold | $Purchased | $TurnoverAmt | $OOS"
} else {
$out = "Daily sales report for $($Date):
- Sold: $Sold
- Purchased: $Purchased
- Turnover: $Turnover
- Out of stocks: $OOS"
	
}
return $out 	
}

#Unique visitors
#Repeat visits

Function Get-ItemsPurchased {
	Param(
		$LogFile = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\latest.log",
		$Data = (Get-MCShopData -LogFile $log),
		$GroupedData = ($Data | Group-Object ItemName | where {$_.count -gt 1} | sort count)
	)
	($GroupedData | 
	select Name, 
	@{n="Sold";e={$sum=0;$_.Group.stockqty | where {$_ -ne "Full"} | where {$_ -ne "Empty"} | where {$_ -match "-"} | %{$sum += $_};$sum}}, 
	@{n="Purchased";e={$sum=0;$_.Group.stockqty | where {$_ -ne "Full"} | where {$_ -ne "Empty"} | where {$_ -notmatch "-"} | %{$sum += $_};$sum}} |
	select Name, Sold, @{n="SoldPctTotal";e={[Math]::round($_.sold / 160100,2)}}, Purchased, @{n="PurchPctTotal";e={[Math]::round($_.Purchased / 92416,2)}}, @{n="Change";e={[Math]::round($_.Purchased + $_.sold,2)}} | 
	select Name, Sold, SoldPctTotal, Purchased, PurchPctTotal, Change, @{n="ChangePctTotal";e={[Math]::round($_.Change / 67684,2)}} | 
	sort purchased -Descending)[0..10] | ft
}

Function Get-ItemsSold {
	Param(
		$LogFile = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\latest.log",
		$Data = (Get-MCShopData -LogFile $log),
		$GroupedData = ($Data | Group-Object ItemName | where {$_.count -gt 1} | sort count)
	)
	($GroupedData | 
	select Name, 
	@{n="Sold";e={$sum=0;$_.Group.stockqty | where {$_ -ne "Full"} | where {$_ -ne "Empty"} | where {$_ -match "-"} | %{$sum += $_};$sum}}, 
	@{n="Purchased";e={$sum=0;$_.Group.stockqty | where {$_ -ne "Full"} | where {$_ -ne "Empty"} | where {$_ -notmatch "-"} | %{$sum += $_};$sum}} |
	select Name, Sold, @{n="SoldPctTotal";e={[Math]::round($_.sold / 160100,2)}}, Purchased, @{n="PurchPctTotal";e={[Math]::round($_.Purchased / 92416,2)}}, @{n="Change";e={[Math]::round($_.Purchased + $_.sold,2)}} | 
	select Name, Sold, SoldPctTotal, Purchased, PurchPctTotal, Change, @{n="ChangePctTotal";e={[Math]::round($_.Change / 67684,2)}} | 
	sort sold )[0..10] | ft
}

Function Get-MoneyMade {
	Param(
		$day = (get-date -f dd),
		$date = (get-date (Get-Date -day $day).AddDays(-1) -f "yyyy-MM-dd"),
		$logfolder = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\",
		[switch]$NoFormat
	)
	ls $logfolder | where {$_.fullname -match "$date"} | where {$_.fullname -match "gz"} | %{DeGZip-File -infile $_}
	$inputfiles = (ls $logfolder | where {$_.fullname -match "$date"} | where {$_.fullname -notmatch "gz"})
	$data = ($inputfiles | %{ (Get-Content $_)})
	$sum = 0;
	($data| where {$_ -match "and you earned"}) -replace " \(","" |%{$sum += [float]($_ -split "\$")[1]};
	$sum = '{0:N}' -f $sum
	if ($NoFormat) {
		$Sum
	} else {
		Write-Host "$($date): Alcove made `$$sum"
	}
}

<#
Date | Sold | Purchased | Turnover | OOS | Revenue
April 16 | 95,699 | 49,754 | 45,945 | 61 | 275,278.18
April 17 | 72,282 | 65,624 | 6,658 | 50 | 273,004.20
April 18 | 128,816 | 93,565 | 35,251 | 73 | 407,935.85
April 19 | 162,649 | 90,096 | 72,553 | 111 | 623,050.98
April 20 | 114,496 | 101,518 | 12,978 | 84 | 510,776.68
April 21 | 96,611 | 57,196 | 39,415 | 66 | 382,364.33
April 22 | 68,083 | 69,302 | 1,219 | 54 | 268,337.23
April 23 | 77,432 | 60,387 | 17,045 | 58 | 233,701.20
April 24 | 79,558 | 53,045 | 26,513 | 43 | 285,722.24
April 25 | 49,978 | 37,698 | 12,280 | 33 | 156,484.18
April 26 | 63,997 | 45,208 | 18,789 | 60 | 333,840.63
April 27 | 137,939 | 76,427 | 61,512 | 79 | 469,329.27
April 28 | 85,971 | 56,169 | 29,802 | 66 | 322,764.66
April 29 | 103,613 | 77,929 | 25,684 | 58 | 408,888.74
April 30 | 81,401 | 72,493 | 8,908 | 58 | 260,474.39
#>

Function Get-Times {
$out = @()
$times = "" | select location, time;$times.location = "China";$times.time =  get-date ([System.TimeZoneInfo]::ConvertTimeBySystemTimeZoneId((Get-Date), 'China Standard Time')) -f t;$out += $times;
$times = "" | select location, time;$times.location = "Redmond";$times.time =  get-date ([System.TimeZoneInfo]::ConvertTimeBySystemTimeZoneId((Get-Date), 'Pacific Standard Time')) -f t;$out += $times;
$times = "" | select location, time;$times.location = "UTC";$times.time =  get-date ([System.TimeZoneInfo]::ConvertTimeBySystemTimeZoneId((Get-Date), 'UTC')) -f t;$out += $times;

$a = ($out | select @{n="time2";e={[int]($_.time -split ":")[0]}}).time2 | sort
$list = foreach ($b in $a) {($out | where {[int]($_.time -split ":")[0] -eq $b.tostring()})}
$listout = ""
$list | %{$listout += $_.time + " " + $_.location + " | "}
$lo = $listout -split " "
$listout = $lo[0..($lo.count -3)] -join " "
return $listout
}

Function Get-NextProdRun {
	Get-Date (Get-Date).AddDays(1) -f "M/dd HH:mm"
}

Function Get-Stacks {
	Param(
		$a = 176,
		$b = ($a/64),
		$c = ([math]::floor($b)),
		[int]$d = (($b - $c) * 64)
	)
	"$a items is $c stacks and $d items."
}

Function Get-NewPlayerTimeToFirstVisit {
	Param(
		$LogFile = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\latest.log",
		$LogData = (gc $LogFIle),
		$NewPlayers = ($LogData | select-string "\+ NEW" )
	)
	#foreach ($player in $newplayers) {$player = ($player -split "NEW ")[1];$player;gc "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\2025-05-10-1.log" | Select-String $player | Select-String "from your shop"}

	foreach ($player in $newplayers) {
		$playername = ($player -split "\+ NEW ")[1];
		$JoinTime = get-date ($player -split "\[" -split "\]")[1]
		$Purchases = $LogData | Select-String $playername | Select-String "from your shop"
		$AvgAmt = 0
		$Count = 0
		if ($Purchases) {
			$FirstPurchase = $null
			if ($Purchases.gettype().BaseType.name -eq "Array") {
				$FirstPurchase = $Purchases[0]
			} elseif ($Purchases.gettype().name -eq "MatchInfo") {
				$FirstPurchase = $Purchases
			}
			$FirstBuyTime = get-date ($FirstPurchase -split "\[" -split "\]")[1]
			$TimeBetweenJoinAndFirstBuy = [math]::round(($FirstBuyTime - $JoinTime).TotalMinutes,2)
			$out = "" | select Name, JoinTime, FirstBuyTime, TimeBetweenJoinAndFirstBuy
			$out.Name = $PlayerName
			$out.JoinTIme = $JoinTIme
			$out.FirstBuyTime = $FirstBuyTime
			$out.TimeBetweenJoinAndFirstBuy = $TimeBetweenJoinAndFirstBuy
			#"Player $playername joined at $JoinTIme and made their first purchase $TimeBetweenJoinAndFirstBuy minutes later."
			Return $out
		}
	}
<#
#>	
}


Function Get-NewPlayerFirstVisitData {
	Param(
		$day = (get-date -f dd),
		$date = (get-date (Get-Date -day $day).AddDays(-1) -f "yyyy-MM-dd"),
		$logfolder = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\",
		[switch]$NoFormat
	)
	ls $logfolder | where {$_.fullname -match "$date"} | where {$_.fullname -match "gz"} | %{DeGZip-File -infile $_}
	$inputfiles = (ls $logfolder | where {$_.fullname -match "$date.$log"} | where {$_.fullname -notmatch "gz"})
	
	$out = $null
	
	try {$inputfiles.FullName | %{$out += Get-NewPlayerTimeToFirstVisit $_ }	} catch {}
	Return $out
}

Function Get-AvgTimeBeforeFirstBuy {
	Param(
		$day = (get-date -f dd),
		$Times = (Get-NewPlayerFirstVisitData $day).TimeBetweenJoinAndFirstBuy
	)
	$Times | %{$sum += $_}
	$Avg = $sum/($Times.count)
	
	return "Day $day had $sum / $($Times.count) for avg $avg"
}

Function Get-CircuitTimer {
	$latest = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\latest.log"
	$data = gc $latest 
#Get the "use again" line number
#Look above for the last "teleporting to" above it
#Use that as the item name.

	$outdata = @()
	$matchdata = $data | where {$_ -match "You can use this again"}
	foreach ($line in $matchdata) {
	try {
		
		$splitline = $line -replace "\[","" -replace "\]","" -split " "
		$out = "" | select Time,Name
		
		$SplitForLineNo = $line -split "CHILLSMP ▶"
		$lineno = ($data | Select-String $SplitForLineNo[1]).LineNumber
		$out.Name = (($data[0..$lineno] | Select-String "Teleporting to circuit")[-1] -replace "\.","" -split "_")[1]
		
		if ($line -match "Hours") {
			$out.Time = [math]::round(((get-date $splitline[0]).AddHours($splitline[13]).AddMinutes($splitline[15]).AddSeconds($splitline[17]) - (get-date)).totalminutes,2) #| where {$_ -gt 0}
		} else {
			$out.Time = [math]::round((((get-date $splitline[0]).AddMinutes($splitline[13]).AddSeconds($splitline[16])) - (get-date)).totalminutes,2) #| where {$_ -gt 0}
		}
		$outdata += $out
	} catch {}
	}
	$outdata  | sort time -Descending | sort name -Unique | sort time 
}

Function Get-PWOwners {
	Param(
		$Logfile = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\latest.log",
		$Data = (gc $Logfile | where {$_ -match " pw "})
	)
	$outdata = @()
	foreach ($Line in $Data) {
		$Line = $Line -split " "
		$out = "" | select PlayerName,PWName
		$out.PlayerName = ($Line | where {$_ -notmatch "\]"} | where {$_ -notmatch "\["})[0]
		$out.PWName = $Line[($Line.IndexOf("pw") +1)]
		$outdata += $out 
	}
	$outdata | group PlayerName | select @{n="PlayerName";e={$_.name}}, @{n="PWList";e={$_.group | group PWName | select name, count}}
}

