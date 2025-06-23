//Copyright 2025 Gilgamech Technologies
//Author: Stephen Gillie
//Created 06/22/2025
//Updated 06/22/2025
//Alcove was the new finditem.





//////////////////////////////////////////====================////////////////////////////////////////
//////////////////////====================--------------------====================////////////////////
//====================--------------------      Init vars     --------------------====================//
//////////////////////====================--------------------====================////////////////////
//////////////////////////////////////////====================////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ShopBotNamespace {
    public class AlcoveShopBot : Form {
//{ Ints
        public int build = 16;//Get-RebuildCsharpApp AlcoveShopBot
		public string appName = "AlcoveShopBot";
		public static string StoreName = "Alcove";
		public string appTitle = StoreName + " Shop Bot - Build ";

        public Button runButton, stopButton;
		//public TextBox webHookBox = new TextBox();
		public TextBox moneyMadeBox = new TextBox();
		public RichTextBox outBox = new RichTextBox();
		public System.Drawing.Bitmap myBitmap;
		public System.Drawing.Graphics pageGraphics;
		public ContextMenuStrip contextMenu1;
		
		public string[] parsedHtml = new string[1];
		public bool WebhookPresent = false;
		public string webHook = "Webhook_Goes_Here";
		
		public static string MainFolder = "C:\\Users\\Gilgamech\\AppData\\Roaming\\.minecraft\\";
		public static string logFolder = MainFolder+"\\logs"; //Logs folder;
		public string LatestLog = logFolder+"\\latest.log";
		
		//ui
		public Panel pagePanel;
		public int displayLine = 0;
		public int sideBufferWidth = 0;
			//outBox.Font = new Font("Calibri", 14);
		
		//Grid
		public static int gridItemWidth = 60;
		public static int gridItemHeight = 30;
		
		public static int row0 = gridItemHeight*0;
		public static int row1 = gridItemHeight*1;
		public static int row2 = gridItemHeight*2;
		public static int row3 = gridItemHeight*3;
		public static int row4 = gridItemHeight*4;
 		public static int row5 = gridItemHeight*5;
 		public static int row6 = gridItemHeight*6;
 		public static int row7 = gridItemHeight*7;
 		public static int row8 = gridItemHeight*8;
 		public static int row9 = gridItemHeight*9;
 		public static int row10 = gridItemHeight*10;
 			
 		public static int col0 = gridItemWidth*0;
 		public static int col1 = gridItemWidth*1;
 		public static int col2 = gridItemWidth*2;
 		public static int col3 = gridItemWidth*3;
 		public static int col4 = gridItemWidth*4;
 		public static int col5 = gridItemWidth*5;
 		public static int col6 = gridItemWidth*6;
 		public static int col7 = gridItemWidth*7;
 		public static int col8 = gridItemWidth*8;
 		public static int col9 = gridItemWidth*9;
 		public static int col10 = gridItemWidth*10;

		public int WindowWidth = col7+20;
		public int WindowHeight = row8+10;
		
		public bool debuggingView = false;
		public string FullText = "is now full";//[05:33:18] [Render thread/INFO]: [System] [CHAT] SHOPS ? Your shop at 15274, 66, 20463 is now full.
		public string SellText = "to your shop";//[05:40:12] [Render thread/INFO]: [System] [CHAT] SHOPS ? kota490 sold 1728 Sea Lantern to your shop {3}.
		public string BuyText = "from your shop";//[05:47:12] [Render thread/INFO]: [System] [CHAT] SHOPS ? _Blackjack29313 purchased 2 Grindstone from your shop and you earned $9.50 ($0.50 in taxes).
		public string EmptyText = "has run out of";//[06:07:40] [Rend
		//public void OldDate = get-date -f dd

		public string DataFile = "C:\\Users\\StephenGillie\\Documents\\PowerShell\\AlcoveShopBot.csv";
		public string OwnerList = "C:\\Users\\StephenGillie\\Documents\\PowerShell\\ChillPWOwnerList.csv";

/*
 		public void Values = @{}
 		public void Values.Empty = "Empty"
 		public void Values.Full = "Full"
 		public void Values.Sell = "Sell"
 		public void Values.Buy = "Buy"
 		public void Values.Neg = "-"
*/





//////////////////////////////////////////====================////////////////////////////////////////
//////////////////////====================--------------------====================////////////////////
//====================--------------------    Boilerplate     --------------------====================//
//////////////////////====================--------------------====================////////////////////
//////////////////////////////////////////====================////////////////////////////////////////
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new AlcoveShopBot());
        }// end Main

        public AlcoveShopBot() {
/* 			if (WebhookPresent == false) {
				CheckWebhook();
			} */
			this.Text = appTitle + build;
			this.Size = new Size(WindowWidth,WindowHeight);
			//this.StartPosition = FormStartPosition.CenterScreen;
			this.Resize += new System.EventHandler(this.OnResize);
			this.AutoScroll = true;
			Icon icon = Icon.ExtractAssociatedIcon("C:\\repos\\AlcoveShopBot\\AlcoveShopBot.ico");
			this.Icon = icon;
			buildMenuBar();
			drawTextBox(ref moneyMadeBox, col0, row0, col2, 0,"Money Made");
 			drawButton(ref runButton, col5, row0, col1, row1, "Run", runButton_Click);
 			drawButton(ref stopButton, col6, row0, col1, row1, "Stop", stopButton_Click);
			drawRichTextBox(ref outBox, col0,row1,col7,row5,"Transaction Log", "outBox");

			//webHookBox.Font = new Font("Calibri", 14);
			moneyMadeBox.Font = new Font("Calibri", 14);
			outBox.Multiline = true;
			outBox.AcceptsTab = true;
			outBox.WordWrap = true;
			outBox.ReadOnly = true;
			outBox.DetectUrls = true;
			//outBox.LinkClicked  += new LinkClickedEventHandler(Link_Click);
			outBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			//webHookBox.KeyUp += webHookBox_KeyUp;

        } // end AlcoveShopBot





//////////////////////////////////////////====================////////////////////////////////////////
//////////////////////====================--------------------====================////////////////////
//====================--------------------      Main     --------------------====================//
//////////////////////====================--------------------====================////////////////////
//////////////////////////////////////////====================////////////////////////////////////////
		public void buildMenuBar (){
			this.Menu = new MainMenu();
			
			MenuItem item = new MenuItem("File");
			this.Menu.MenuItems.Add(item);
				item.MenuItems.Add("Open log folder", new EventHandler(Open_Log_Folder)); 
			
			item = new MenuItem("Edit");
			this.Menu.MenuItems.Add(item);
				item.MenuItems.Add("Shop name", new EventHandler(Edit_Shop_Name));
				item.MenuItems.Add("Shop coordinates", new EventHandler(Edit_Shop_Coords)); 
				item.MenuItems.Add("Webhook", new EventHandler(Edit_Webhook)); 
			
			item = new MenuItem("Reports");
			this.Menu.MenuItems.Add(item);
				item.MenuItems.Add("Daily", new EventHandler(Daily_Report));
				item.MenuItems.Add("Weekly", new EventHandler(Weekly_Report));
				item.MenuItems.Add("Monthly", new EventHandler(Monthly_Report));
				item.MenuItems.Add("Other", new EventHandler(Other_Report));
			
			item = new MenuItem("Help");
			this.Menu.MenuItems.Add(item);
				item.MenuItems.Add("About", new EventHandler(About_Click));
	   }// end buildMenuBar
		





//////////////////////////////////////////====================////////////////////////////////////////
//////////////////////====================--------------------====================////////////////////
//====================--------------------   UI templates    --------------------====================//
//////////////////////====================--------------------====================////////////////////
//////////////////////////////////////////====================////////////////////////////////////////
		public void drawButton(ref Button button, int pointX, int pointY, int sizeX, int sizeY,string buttonText, EventHandler buttonOnclick){
			button = new Button();
			button.Text = buttonText;
			button.Location = new Point(pointX, pointY);
			button.Size = new Size(sizeX, sizeY);
			//button.BackColor = color_DefaultBack;
			//button.ForeColor = color_DefaultText;
			button.Click += new EventHandler(buttonOnclick);
			// button.Font = new Font(buttonFont, buttonFontSIze);
			Controls.Add(button);
		}// end drawButton

		public void drawRichTextBox(ref RichTextBox richTextBox, int pointX,int pointY,int sizeX,int sizeY,string text, string name){
			richTextBox = new RichTextBox();
			richTextBox.Text = text;
			richTextBox.Name = name;
			richTextBox.Multiline = true;
			richTextBox.AcceptsTab = true;
			richTextBox.WordWrap = true;
			richTextBox.ReadOnly = true;
			richTextBox.DetectUrls = true;
			// richTextBox.BackColor = color_DefaultBack;
			// richTextBox.ForeColor = color_DefaultText;
			// richTextBox.Font = new Font(AppFont, AppFontSIze);
			richTextBox.Location = new Point(pointX, pointY);
			//richTextBox.LinkClicked  += new LinkClickedEventHandler(Link_Click);
			richTextBox.Width = sizeX;
			richTextBox.Height = sizeY;
			//richTextBox.Dock = DockStyle.Fill;
			richTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;


			//richTextBox.BackColor = Color.Red;
			//richTextBox.ForeColor = Color.Blue;
			//richTextBox.RichTextBoxScrollBars = ScrollBars.Both;
			//richTextBox.AcceptsReturn = true;

			Controls.Add(richTextBox);
		}// end drawRichTextBox
		
		public void drawTextBox(ref TextBox urlBox, int pointX, int pointY, int sizeX, int sizeY,string text){
			urlBox = new TextBox();
			urlBox.Text = text;
			urlBox.Name = "urlBox";
			// urlBox.Font = new Font(AppFont, urlBoxFontSIze);
			urlBox.Location = new Point(pointX, pointY);
			// urlBox.BackColor = color_InputBack;
			// urlBox.ForeColor = color_DefaultText;
			urlBox.Width = sizeX;
			urlBox.Height = sizeY;
			Controls.Add(urlBox);
		}
		
		public void drawLabel(ref Label newLabel, int pointX, int pointY, int sizeX, int sizeY,string text){
			newLabel = new Label();
			newLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			//newLabel.ImageList = imageList1;
			newLabel.ImageIndex = 1;
			newLabel.ImageAlign = ContentAlignment.TopLeft;
			// newLabel.BackColor = color_DefaultBack;
			// newLabel.ForeColor = color_DefaultText;
			newLabel.Name = "newLabel";
			// newLabel.Font = new Font(AppFont, AppFontSIze);
			newLabel.Location = new Point(pointX, pointY);
			newLabel.Width = sizeX;
			newLabel.Height = sizeY;
			//newLabel.KeyUp += newLabel_KeyUp;

			newLabel.Text = text;

			//newLabel.Size = new Size (label1.PreferredWidth, label1.PreferredHeight);
			Controls.Add(newLabel);
		}

		public void drawDataGrid(ref DataGridView dataGridView, int startX, int startY, int sizeX, int sizeY){
			dataGridView = new DataGridView();
			dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
			dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
			dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			
			// dataGridView.ForeColor = color_DefaultText;//Selected cell text color
			// dataGridView.BackColor = color_DefaultBack;//Selected cell BG color
			// dataGridView.DefaultCellStyle.SelectionForeColor  = color_DefaultText;//Unselected cell text color
			// dataGridView.DefaultCellStyle.SelectionBackColor = color_DefaultBack;//Unselected cell BG color
			// dataGridView.BackgroundColor = color_DefaultBack;//Space underneath/between cells
			dataGridView.GridColor = SystemColors.ActiveBorder;//Gridline color
			
			dataGridView.Name = "dataGridView";
			// dataGridView.Font = new Font(AppFont, AppFontSize);
			dataGridView.Location = new Point(startX, startY);
			dataGridView.Size = new Size(sizeX, sizeY);
			// dataGridView.KeyUp += dataGridView_KeyUp;
			// dataGridView.Text = text;
			Controls.Add(dataGridView);


		
			dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
			dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
			dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dataGridView.AllowUserToDeleteRows = false;
			dataGridView.RowHeadersVisible = false;
			dataGridView.MultiSelect = false;
			//dataGridView.Dock = DockStyle.Fill;

/*
			dataGridView.CellFormatting += new DataGridViewCellFormattingEventHandler(dataGridView_CellFormatting);
			dataGridView.CellParsing += new DataGridViewCellParsingEventHandler(dataGridView_CellParsing);
			addNewRowButton.Click += new EventHandler(addNewRowButton_Click);
			deleteRowButton.Click += new EventHandler(deleteRowButton_Click);
			ledgerStyleButton.Click += new EventHandler(ledgerStyleButton_Click);
			dataGridView.CellValidating += new DataGridViewCellValidatingEventHandler(dataGridView_CellValidating);
*/
		}// end drawDataGrid

		public void drawToolTip(ref ToolTip toolTip, ref Button button, string DisplayText, int AutoPopDelay = 5000, int InitialDelay = 1000, int ReshowDelay = 500){
			toolTip = new ToolTip();

			// Set up the delays for the ToolTip.
			toolTip.AutoPopDelay = AutoPopDelay;
			toolTip.InitialDelay = InitialDelay;
			toolTip.ReshowDelay = ReshowDelay;
			// Force the ToolTip text to be displayed whether or not the form is active.
			toolTip.ShowAlways = true;
			 
			// Set up the ToolTip text for the Button and Checkbox.
			toolTip.SetToolTip(button, DisplayText);
			//toolTip.SetToolTip(this.checkBox1, "My checkBox1");
		}

		public void drawStatusStrip (StatusStrip statusStrip,ToolStripStatusLabel toolStripStatusLabel) {
			statusStrip = new System.Windows.Forms.StatusStrip();
			statusStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            statusStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            
			toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new System.Drawing.Size(109, 17);
            toolStripStatusLabel.Text = "toolStripStatusLabel";
			statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel });
            
            statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            statusStrip.Location = new System.Drawing.Point(0, 0);
            statusStrip.Name = "statusStrip";
            statusStrip.ShowItemToolTips = true;
            statusStrip.Size = new System.Drawing.Size(292, 22);
            statusStrip.SizingGrip = false;
            statusStrip.Stretch = false;
            statusStrip.TabIndex = 0;
            statusStrip.Text = "statusStrip";
			
			Controls.Add(statusStrip);
		}
		






//////////////////////////////////////////====================////////////////////////////////////////
//////////////////////====================--------------------====================////////////////////
//====================--------------------      ShopBot     --------------------====================//
//////////////////////====================--------------------====================////////////////////
//////////////////////////////////////////====================////////////////////////////////////////
/* ShopBot
Operation:
- If Run button is clicked: 
  - If webhook bar populated with URL, reads latest.log every 10 seconds and sends the output to the URL.
  - Updates "money made this session" display.
- If Stop button is clicked: 
  - Nothing does anything.


*/

		//Data
		public void DailyData (int setzero) {
		/*
			Param(
				day = (Get-Date (Get-Date).AddDays(-1) -f dd),
				date = (get-date -Day day -f "yyyy-MM-dd"),
				logfolder = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\",
				[Switch]Silent
			)
			UnzipFiles = ls logfolder | where {_.fullname -match "date"} | where {_.fullname -match "gz"}
			Foreach (Filename in UnzipFiles) {
					DeGZip(Filename)
			}
			inputfiles = ls logfolder | where {_.fullname -match "date"} | where {_.fullname -notmatch "gz"}
			
			Data = null
			inputfiles | %{
				if (!Silent) {
					Write-Host "Reading _"
				}
				Data += (GetShopData -LatestLog _)
			}
			return Data
		}
		*/
		}

		public void GetShopData (int setzero) {
		/*
			Data = (((gc LatestLog) -split "`n" | select-string "SHOPS ? " | Where {_ -notmatch "Enter all in chat"}) -replace "\[Render thread/INFO\]: \[System\] \[CHAT\] SHOPS ? ","")
			EmptyText = "*has run out of*"#[06:07:40] Your shop at 15226, 63, 20487 has run out of Cocoa Beans!
			#[07:12:51] [Render thread/INFO]: [System] [CHAT] SHOPS ? This shop has run out of space! Contact the shop owner or staff to get it emptied.
			FullText = "*is now full*"#[05:33:18] Your shop at 15274, 66, 20463 is now full.
			SellText = "*to your shop*" #[05:40:12] kota490 sold 1728 Sea Lantern to your shop {3}.
			BuyText = "*from your shop*" #[05:47:12] _Blackjack29313 purchased 2 Grindstone from your shop and you earned 9.50 (0.50 in taxes).
			NameData = gc dataFile | convertfrom-csv
			out = @()
			n = 0
			Data = Data | where {_ -notmatch "out of space"}
			Data = Data | where {_ -notmatch "how many you wish"}
			foreach (Item in Data) {
				n++
				new = "" | select ItemName,XLoc,YLoc,ZLoc,StockQty,Event,DateTime,PlayerName
				#Use all 3 coords because some places have 2 buy shops stacked at the same X and Z. 
				switch -wildcard (Item) {
					EmptyText {
						SplitItem = Item -replace "\[","" -replace "\] Your shop at","," -replace " has run out of","," -replace "!","" -split ", "
						#06:07:40 ,15226, 63, 20487, Cocoa Beans
						new.DateTime = SplitItem[0]
						new.XLoc = SplitItem[1]
						new.YLoc = SplitItem[2]
						new.ZLoc = SplitItem[3]
						new.ItemName = SplitItem[4]
						new.Event = "Empty"
						new.StockQty = (data[data.IndexOf(item)-1] -split " ")[3]
						new.PlayerName = (data[data.IndexOf(item)-1] -split " ")[1]
					}
					FullText {
						SplitItem = Item -replace "\[","" -replace "\] Your shop at","," -replace " is now full.",", " -replace "!","" -split ", "
						#05:33:18, 15274, 66, 20463,
						new.DateTime = SplitItem[0]
						new.XLoc = SplitItem[1]
						new.YLoc = SplitItem[2]
						new.ZLoc = SplitItem[3]
						#new.ItemName = SplitItem[4]
						new.Event = "Full"
						new.StockQty = (data[data.IndexOf(item)-1] -split " ")[3]
						new.PlayerName = (data[data.IndexOf(item)-1] -split " ")[1]
					}
					SellText {
					try {
						SplitItem = Item -replace "\[","" -replace "\]","," -replace " sold ",", " -replace " to your shop \{3\}.",", " -replace "!","" -split ", "
						 #05:40:12, kota490, 1728 Sea Lantern,
						new.DateTime = SplitItem[0]
						new.PlayerName = SplitItem[1]
						#new.XLoc = SplitItem[1]
						#new.YLoc = SplitItem[2]
						#new.ZLoc = SplitItem[3]
						new.ItemName = (SplitItem[2] -split " ")[1..9] -join " "
						new.StockQty = [int](SplitItem[2] -split " ")[0]
						new.Event = "Sell"
						new.PlayerName = SplitItem[1]
						} catch {}
					}
					BuyText {
						SplitItem = Item -replace "\[","" -replace "\]","," -replace " purchased ",", " -replace " from your shop and you earned ",", " -replace "\(","" -split ", "
						#05:47:12 _Blackjack29313, 2 Grindstone, 9.50 0.50 in taxes).
						new.DateTime = SplitItem[0]
						new.PlayerName = SplitItem[1]
						#new.XLoc = SplitItem[1]
						#new.YLoc = SplitItem[2]
						#new.ZLoc = SplitItem[3]
						new.ItemName = (SplitItem[2] -split " ")[1..9] -join " "
						new.StockQty = [int](-(SplitItem[2] -split " ")[0])
						new.Event = "Buy"
						new.PlayerName = SplitItem[1]
					}
				}#end switch
				pct = [Math]::round((n/data.length)*100)
				Write-Progress -Activity "Reading latest.log" -Status "Item n of (data.length) ((pct)% complete)- item" -PercentComplete pct
				
				NewName = NameData | where {_.Xloc -eq new.Xloc} | where {_.Yloc -eq new.Yloc} | where {_.Zloc -eq new.Zloc}
				if (NewName) {
					new.ItemName = NewName.ItemName
				}
				
				out += new
			}#end foreach
			out
		*/
		}

		//Sales
		public void ItemsPurchased (int setzero) {
		/*
			GroupedData = (GetShopData() | Group-Object ItemName | where {_.count -gt 1} | sort count)
			(GroupedData | 
			select Name, 
			@{n="Sold";e={sum=0;_.Group.stockqty | where {_ -ne "Full"} | where {_ -ne "Empty"} | where {_ -match "-"} | %{sum += _};sum}}, 
			@{n="Purchased";e={sum=0;_.Group.stockqty | where {_ -ne "Full"} | where {_ -ne "Empty"} | where {_ -notmatch "-"} | %{sum += _};sum}} |
			select Name, Sold, @{n="SoldPctTotal";e={[Math]::round(_.sold / 160100,2)}}, Purchased, @{n="PurchPctTotal";e={[Math]::round(_.Purchased / 92416,2)}}, @{n="Change";e={[Math]::round(_.Purchased + _.sold,2)}} | 
			select Name, Sold, SoldPctTotal, Purchased, PurchPctTotal, Change, @{n="ChangePctTotal";e={[Math]::round(_.Change / 67684,2)}} | 
			sort purchased -Descending)[0..10] | ft
		}
		*/
		}

		public void ItemsSold (int setzero) {
		/*
			Data = ,
			GroupedData = (GetShopData() | Group-Object ItemName | where {_.count -gt 1} | sort count)
			(GroupedData | 
			select Name, 
			@{n="Sold";e={sum=0;_.Group.stockqty | where {_ -ne "Full"} | where {_ -ne "Empty"} | where {_ -match "-"} | %{sum += _};sum}}, 
			@{n="Purchased";e={sum=0;_.Group.stockqty | where {_ -ne "Full"} | where {_ -ne "Empty"} | where {_ -notmatch "-"} | %{sum += _};sum}} |
			select Name, Sold, @{n="SoldPctTotal";e={[Math]::round(_.sold / 160100,2)}}, Purchased, @{n="PurchPctTotal";e={[Math]::round(_.Purchased / 92416,2)}}, @{n="Change";e={[Math]::round(_.Purchased + _.sold,2)}} | 
			select Name, Sold, SoldPctTotal, Purchased, PurchPctTotal, Change, @{n="ChangePctTotal";e={[Math]::round(_.Change / 67684,2)}} | 
			sort sold )[0..10] | ft
		*/
		}

		//Discord
		public void SendDiscordMessage (string content) {
			string payload = "{\"content\": \"" + content + "\"}";
			//InvokeWebRequest(webHook, WebRequestMethods.Http.Post, payload,false,true);
			InvokeWebRequest(moneyMadeBox.Text, WebRequestMethods.Http.Post, payload,false,true);
		}

		//Automation
		public void RunBot (int setzero) {
			string OldData = "";
			while (true) {
		/*
				ShopData = (GetShopData | where {_.event -match "Empty"});
				content = try{(diff OldData ShopData).inputobject}catch{}
				#content = (diff ShopData OldData -ErrorAction SilentlyContinue).inputobject;
				foreach (item in content){
				#if (content){
					out = "Empty shop: Player ``(item.PlayerName)`` has purchased the last (item.StockQty) (item.ItemName)!";
					out
					Send-DiscordMessage -content out #-WhatIf
				};
				OldData = ShopData

				sleep 10
			}
		*/
			}
		}

		//Reports
		public void MoneyMade (int setzero) {
		/*
			Param(
				day = (get-date -f dd),
				date = (get-date (Get-Date -day day).AddDays(-1) -f "yyyy-MM-dd"),
				logfolder = "C:\Users\StephenGillie\AppData\Roaming\.minecraft\logs\",
				[switch]NoFormat
			)
			ls logfolder | where {_.fullname -match "date"} | where {_.fullname -match "gz"} | %{DeGZip-File -infile _}
			inputfiles = (ls logfolder | where {_.fullname -match "date"} | where {_.fullname -notmatch "gz"})
			data = (inputfiles | %{ (Get-Content _)})
			sum = 0;
			(data| where {_ -match "and you earned"}) -replace " \(","" |%{sum += [float](_ -split "\")[1]};
			sum = '{0:N}' -f sum
			if (NoFormat) {
				Sum
			} else {
				Write-Host "(date): Alcove made `sum"
			}
		*/
		}

		public void BuildDailyReport (int setzero, bool Weekly = false) {
		/*
			Param(
				day = (Get-Date (Get-Date).AddDays(-1) -f dd),
				date = (get-date -Day day -f "yyyy-MM-dd"),
			)
			Data = Get-DailyData -day day -date date -Silent
			if (Weekly) {
				GetDailySales -Data Data -Date (Get-Date (get-date date) -Format "MMMM d") -Weekly
			} else {
				GetDailySales -Data Data -Date (Get-Date (get-date date) -Format "MMMM d")
			}
		*/
		}

		public void BuildWeeklyReport (int days = 14) {
		/*
			"Date | Sold | Purchased | Turnover | OOS | Revenue"
			(days +1)..1 | %{
				date = (get-date (get-date).AddDays(-_) -f "yyyy-MM-dd")
				
				Report = Get-BuildDailyReport -date date -Silent -Weekly;
				MoneyMade = Get-MoneyMade -date date -NoFormat
				Report + " | MoneyMade"
			}
		*/
		}

		public void GetDailySales (int setzero) {
		/*
				Data = (GetShopData -LatestLog LatestLog | where {_.event}),
				Date = (Get-Date (get-date).AddDays(-1) -Format "MMMM d"),
				[switch]Weekly

			Sold = 0
			Data | where {_.event -match Values.Buy}  | %{
				try {
					Sold += (_.stockqty -replace "-","")
				} catch {}
			}
			
			Purchased = 0
			Data | where {_.event -match Values.Sell}  | %{
				try {
					Purchased += (_.stockqty -replace "-","")
				} catch {}
			}
			
			If (Purchased -gt Sold) {
				TurnoverAmt = Purchased - Sold
				TurnoverAmt = '{0:N0}' -f TurnoverAmt
				Sold = '{0:N0}' -f Sold
				Purchased = '{0:N0}' -f Purchased
				Turnover = "TurnoverAmt more purchased than sold." 
			} elseif (Purchased -lt Sold) {
				TurnoverAmt = Sold - Purchased
				TurnoverAmt = '{0:N0}' -f TurnoverAmt
				Sold = '{0:N0}' -f Sold
				Purchased = '{0:N0}' -f Purchased
				Turnover = "TurnoverAmt more sold than purchased." 
			} else {
				Turnover = "Exact same amount purchased as sold. (This is rare, please double-check.)" 
			}
			OOS = (data | where {_.Event -match "Empty"}).count
			#OOS = (data.StockQty | where {_EVent -match Values.Empty}).count
			
		out = ""
		if (Weekly) {
		out = "(Date) | Sold | Purchased | TurnoverAmt | OOS"
		} else {
		out = "Daily sales report for (Date):
		- Sold: Sold
		- Purchased: Purchased
		- Turnover: Turnover
		- Out of stocks: OOS"
			
		}
		return out 	
		}
		*/
		}

/* 		//Webhook
		public void CheckWebhook (int setzero) {
				webHook = webHookBox.Text;
				if (webHook.Length > 0) {
					WebhookPresent = true;
				} else {
					WebhookPresent = false;
				}
		}
 */



//////////////////////////////////////////====================////////////////////////////////////////
//////////////////////====================--------------------====================////////////////////
//===================--------------------   Utility Functions  --------------------====================
//////////////////////====================--------------------====================////////////////////
//////////////////////////////////////////====================////////////////////////////////////////
		public string findIndexOf(string pageString,string startString,string endString,int startPlus,int endPlus){
			return pageString.Substring(pageString.IndexOf(startString)+startPlus, pageString.IndexOf(endString) - pageString.IndexOf(startString)+endPlus);
        }// end findIndexOf

		public void DeGZip (string infile) {
			string outfile = infile.Replace(".gz","");
			FileStream compressedFileStream = File.Open(infile, FileMode.Open);
			FileStream outputFileStream = File.Create(outfile);
			var decompressor = new GZipStream(compressedFileStream, CompressionMode.Decompress);
			decompressor.CopyTo(outputFileStream);
		}



//////////////////////////////////////////====================////////////////////////////////////////
//////////////////////====================--------------------====================////////////////////
//===================--------------------  PowerShell Equivs  --------------------====================
//////////////////////====================--------------------====================////////////////////
//////////////////////////////////////////====================////////////////////////////////////////
/*Powershell functional equivalency imperatives
		Get-Clipboard = Clipboard.GetText();
		Get-Date = DateTime.Now.ToString("M/d/yyyy");
		Get-Process = public Process[] processes = Process.GetProcesses(); or var processes = Process.GetProcessesByName("Test");
		New-Item = Directory.CreateDirectory(Path) or File.Create(Path);
		Remove-Item = Directory.Delete(Path) or File.Delete(Path);
		Get-ChildItem = string[] entries = Directory.GetFileSystemEntries(path, "*", SearchOption.AllDirectories);
		Start-Process = System.Diagnostics.Process.Start("PathOrUrl");
		Stop-Process = StopProcess("ProcessName");
		Start-Sleep = Thread.Sleep(GitHubRateLimitDelay);
		Get-Random - Random rnd = new Random(); or int month  = rnd.Next(1, 13);  or int card   = rnd.Next(52);
		Create-Archive = ZipFile.CreateFromDirectory(dataPath, zipPath);
		Expand-Archive = ZipFile.ExtractToDirectory(zipPath, extractPath);
		Sort-Object = .OrderBy(n=>n).ToArray(); and -Unique = .Distinct(); Or Array.Sort(strArray); or List
		
		Get-VM = GetVM("VMName");
		Start-VM = SetVMState("VMName", 2);
		Stop-VM = SetVMState("VMName", 4);
		Stop-VM -TurnOff = SetVMState("VMName", 3);
		Reboot-VM = SetVMState("VMName", 10);
		Reset-VM = SetVMState("VMName", 11);
		
*/
		//JSON
/*		public dynamic FromJson(string string_input) {
			dynamic dynamic_output = new System.Dynamic.ExpandoObject();
			dynamic_output = serializer.Deserialize<dynamic>(string_input);
			return dynamic_output;
		}
*/
		
/*		public string ToJson(dynamic dynamic_input) {
			string string_out;
			string_out = serializer.Serialize(dynamic_input);
			return string_out;
		}
*/
		//CSV
		public Dictionary<string, dynamic>[] FromCsv(string csv_in) {
			//CSV isn't just a 2d object array - it's an array of Dictionary<string,object>, whose string keys are the column headers. 
			string[] Rows = csv_in.Replace("\r\n","\n").Replace("\"","").Split('\n');
			string[] columnHeaders = Rows[0].Split(',');
			Dictionary<string, dynamic>[] matrix = new Dictionary<string, dynamic> [Rows.Length];
			try {
				for (int row = 1; row < Rows.Length; row++){
					matrix[row] = new Dictionary<string, dynamic>();
					//Need to enumerate values to create first row.
					string[] rowData = Rows[row].Split(',');
					try {
						for (int col = 0; col < rowData.Length; col++){
							//Need to record or access first row to match with values. 
							matrix[row].Add(columnHeaders[col].ToString(), rowData[col]);
						}
					} catch {
					}
				}
			} catch {
			}
			return matrix;
		}

		public string ToCsv(Dictionary<string, dynamic>[] matrix) {
			string csv_out = "";
			//Arrays seem to have a buffer row above and below the data.
			int topRow = 1;
			Dictionary<string, dynamic> headerRow = matrix[topRow];
			//Write header row (th). Support for multi-line headers maybe someday but not today. 
			if (headerRow != null) {
				string[] columnHeaders = new string[headerRow.Keys.Count];
				headerRow.Keys.CopyTo(columnHeaders, 0);
				//var a = matrix[0].Keys;
				foreach (string columnHeader in columnHeaders){
						csv_out += columnHeader.ToString()+",";
				}
				csv_out = csv_out.TrimEnd(',');
				// Write data rows (td).
				for (int row = topRow; row < matrix.Length -1; row++){
					csv_out += "\n";
					foreach (string columnHeader in columnHeaders){
						csv_out += matrix[row][columnHeader]+",";
					}
					csv_out = csv_out.TrimEnd(',');
				}
			}
			csv_out += "\n";
			return csv_out;
		}
		//File
		public string GetContent(string Filename, bool NoErrorMessage = false) {
			string string_out = "";
			try {
				// Open the text file using a stream reader.
				using (var sr = new StreamReader(Filename)) {
					// Read the stream as a string, and write the string to the console.
					string_out = sr.ReadToEnd();
				}
			} catch (IOException e) {
				if (NoErrorMessage == false) {
					MessageBox.Show(e.Message, "Error");
				}
			}
			return string_out;
		}

		public void OutFile(string path, object content, bool Append = false) {
			//From SO: Use "typeof" when you want to get the type at compilation time. Use "GetType" when you want to get the type at execution time. "is" returns true if an instance is in the inheritance tree.
			if (TestPath(path) == "None") {
				File.Create(path).Close();
			}
			if (content.GetType() == typeof(string)) {
				string out_content = (string)content;
			//From SO: File.WriteAllLines takes a sequence of strings - you've only got a single string. If you only want your file to contain that single string, just use File.WriteAllText.
				if (Append == true) {
					File.AppendAllText(path, out_content, Encoding.ASCII);//string
				} else {
					File.WriteAllText(path, out_content, Encoding.ASCII);//string
				}
			} else {
				IEnumerable<string> out_content = (IEnumerable<string>)content;
				if (Append == true) {
					File.AppendAllLines(path, out_content, Encoding.ASCII);//IEnumerable<string>'
				} else {
					File.WriteAllLines(path, out_content, Encoding.ASCII);//string[]
				}				
			}
		}

		public string TestPath(string path) {
				string string_out = "";
				if (path != null) {
						path = path.Trim();
					if (Directory.Exists(path)) {
						string_out = "Directory";
					} else if (File.Exists(path)) {
						string_out = "File";
					} else {// neither file nor directory exists. guess intention
						string_out = "None";
					}
				} else {// neither file nor directory exists. guess intention
					string_out = "Empty";
				}
				return string_out;
			}
		//Web
		public string InvokeWebRequest(string Url, string Method = WebRequestMethods.Http.Get, string Body = "",bool Authorization = false,bool JSON = false){ 
			string response_out = "";

				// SSL stuff
				//ServicePointManager.Expect100Continue = true;
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
				
				if (JSON == true) {
					request.ContentType = "application/json";
				}
				if (Authorization == true) {
					//request.Headers.Add("Authorization", "Bearer "+webHook);
					//request.Headers.Add("X-GitHub-Api-build", "2022-11-28");
					request.PreAuthenticate = true;
				}

				request.Method = Method;
				request.ContentType = "application/json;charset=utf-8";
				request.Accept = "application/vnd.github+json";
				request.UserAgent = "WinGetApprovalPipeline";

			 //Check Headers
			 // for (int i=0; i < response.Headers.Count; ++i)  {
				// outBox_msg.AppendText(Environment.NewLine + "Header Name : " + response.Headers.Keys[i] + "Header value : " + response.Headers[i]);
			 // }

			try {
				if ((Body == "") || (Method ==WebRequestMethods.Http.Get)) {
				} else {
					var data = Encoding.Default.GetBytes(Body); // note: choose appropriate encoding
					request.ContentLength = data.Length;
					var newStream = request.GetRequestStream(); // get a ref to the request body so it can be modified
					newStream.Write(data, 0, data.Length);
					newStream.Close();
				} 

				} catch (Exception e) {
					//MessageBox.Show("Wrong request!" + ex.Message, "Error");
					response_out = "Request Error: " + e.Message;
				}
				
				try {
					HttpWebResponse response = (HttpWebResponse)request.GetResponse();
					StreamReader sr = new StreamReader(response.GetResponseStream());
					if (Method == "Head") {
						string response_text = response.StatusCode.ToString();
						response_out = response_text;

					} else {
						string response_text = sr.ReadToEnd();
						response_out = response_text;
					}
					sr.Close();
				} catch (Exception e) {
					response_out = "Response Error: " + e.Message;
				}
		return response_out;
		}// end InvokeWebRequest	

		public void RemoveItem(string Path,bool remake = false){
			if (TestPath(Path) == "File") {
				File.Delete(Path);
				if (remake) {
					File.Create(Path);
				}
			} else if (TestPath(Path) == "Directory") {
				Directory.Delete(Path, true);
				if (remake) {
					Directory.CreateDirectory(Path);
				}
			}
		}





//////////////////////////////////////////====================////////////////////////////////////////
//////////////////////====================--------------------====================////////////////////
//===================--------------------    Event Handlers   --------------------====================
//////////////////////====================--------------------====================////////////////////
//////////////////////////////////////////====================////////////////////////////////////////
		//ui
        public void runButton_Click(object sender, EventArgs e) {
			outBox.Text = "Run" + Environment.NewLine + outBox.Text;
			SendDiscordMessage("Run");
        }// end runButton_Click

        public void stopButton_Click(object sender, EventArgs e) {
			outBox.Text = "Stop" + Environment.NewLine + outBox.Text;
			SendDiscordMessage("Stop");
        }// end stopButton_Click

		public void OnResize(object sender, System.EventArgs e) {
		}
		
/* 		public void webHookBox_KeyUp(object sender, KeyEventArgs e) {
			CheckWebhook();
		}

 */
		//Menu
		//File
		public void Open_Log_Folder(object sender, EventArgs e) {
		}// end Open_Log_Folder

		public void Edit_Shop_Name(object sender, EventArgs e) {
		}// end Edit_Shop_Name

		public void Edit_Shop_Coords(object sender, EventArgs e) {
		}// end Edit_Shop_Coords

		public void Edit_Webhook(object sender, EventArgs e) {
		}// end Edit_Webhook
		
		//Reports
		public void Daily_Report(object sender, EventArgs e) {
		}// end Daily_Report
		
		public void Weekly_Report(object sender, EventArgs e) {
		}// end Weekly_Report
		
		public void Monthly_Report(object sender, EventArgs e) {
		}// end Monthly_Report
		
		public void Other_Report(object sender, EventArgs e) {
		}// end Other_Report
		
		//Help
		public void About_Click (object sender, EventArgs e) {
			string AboutText = "Alcove Shop Bot" + Environment.NewLine;
			AboutText += "Generates out-of-stock alerts and financial reports from QuickShop" + Environment.NewLine;
			AboutText += "Buy/Sell comments in Minecraft chat logs. Made for The Alcove player" + Environment.NewLine;
			AboutText += "-run store on the ChillSMP Minecraft server. But this product isn't" + Environment.NewLine;
			AboutText += "affiliated with any of those." + Environment.NewLine;
			AboutText += "" + Environment.NewLine;
			AboutText += "Version 1." + build + Environment.NewLine;
			AboutText += "(C) 2025 Gilgamech Technologies" + Environment.NewLine;
			AboutText += "" + Environment.NewLine;
			AboutText += "Report bugs & request features:" + Environment.NewLine;
			AboutText += "ShopBot@Gilgamech.com" + Environment.NewLine;
			MessageBox.Show(AboutText);
		} // end Link_Click
    }// end AlcoveShopBot
}// end ShopBotNamespace

