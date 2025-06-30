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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Web.Script.Serialization;

namespace ShopBotNamespace {
    public class AlcoveShopBot : Form {
//{ Ints
        public int build = 16;//Get-RebuildCsharpApp AlcoveShopBot
		public string appName = "AlcoveShopBot";
		public static string StoreName = "Alcove";
		public string appTitle = StoreName + " Shop Bot - Build ";
		
		/*
		account = AlcoveShopBot.Properties.Settings.Default.Account;
		AlcoveShopBot.Properties.Settings.Default.Account = account;
		AlcoveShopBot.Properties.Settings.Default.Save( );
		
		textboxPassword_TextChanged(object sender, EventArgs e) {
			if(PostTracker.Properties.Settings.Default.Password || textboxPassword.Text
				// . . . check for other properties can ( and should ) come in here
				) {
					UpdateApplyButton();
					// Your action comes here, for example
			}
		}

		*/

		List<StockItem>  OldData = new List<StockItem>();
		JavaScriptSerializer serializer = new JavaScriptSerializer();
		System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public Button runButton, stopButton,sendButton;
		//public TextBox webHookBox = new TextBox();
		public TextBox moneyMadeBox = new TextBox();
		public RichTextBox outBox = new RichTextBox();
		public System.Drawing.Bitmap myBitmap;
		public System.Drawing.Graphics pageGraphics;
		public ContextMenuStrip contextMenu1;
		
		public string[] parsedHtml = new string[1];
		public bool WebhookPresent = false;
		public string webHook = "https://discord.com/api/webhooks/1386745728314904736/-yTnmKl48zRx-PfSqLV8YIM-lCkZEbKR9kmLfKtGlttvUKeEO7oc3yvERe0Hbrl0YCNF";

		
		// public static string WindowsUsername = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
		// public static string MainFolder = "C:\\Users\\"+WindowsUsername+"\\AppData\\Roaming\\.minecraft\\";
		// public static string logFolder = MainFolder+"\\logs"; //Logs folder;
		public static string logFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.minecraft\\logs";
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

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter
}


		public int WindowWidth = col7+20;
		public int WindowHeight = row8+10;
		
		public bool debuggingView = false;
		public string FullText = "is now full";//[05:33:18] [Render thread/INFO]: [System] [CHAT] SHOPS ▶ Your shop at 15274, 66, 20463 is now full.
		public string SellText = "to your shop";//[05:40:12] [Render thread/INFO]: [System] [CHAT] SHOPS ▶ kota490 sold 1728 Sea Lantern to your shop {3}.
		public string BuyText = "from your shop";//[05:47:12] [Render thread/INFO]: [System] [CHAT] SHOPS ▶ _Blackjack29313 purchased 2 Grindstone from your shop and you earned $9.50 ($0.50 in taxes).
		public string EmptyText = "has run out of";//[06:07:40] [Rend
		//public void OldDate = get-date -f dd

		//public string DataFile = "C:\\repos\\AlcoveShopBot\\AlcoveShopBot.csv";
		//public string OwnerList = "C:\\repos\\AlcoveShopBot\\ChillPWOwnerList.csv";

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
			timer.Interval = (10 * 1000);
			timer.Tick += new EventHandler(timer_everysecond);
			timer.Start();

			this.Text = appTitle + build;
			this.Size = new Size(WindowWidth,WindowHeight);
			this.Resize += new System.EventHandler(this.OnResize);
			this.AutoScroll = true;
			// Icon icon = Icon.ExtractAssociatedIcon("C:\\repos\\AlcoveShopBot\\AlcoveShopBot.ico");
			// this.Icon = icon;
			buildMenuBar();
			drawTextBox(ref moneyMadeBox, col0, row0, col2, 0,"Money Made");
 			drawButton(ref stopButton, col2, row0, col2, row1, "Daily Report", sendButton_Click);
 			drawButton(ref runButton, col5, row0, col1, row1, "Run", runButton_Click);
 			drawButton(ref sendButton, col6, row0, col1, row1, "Stop", stopButton_Click);
			drawRichTextBox(ref outBox, col0,row1,col7,row5,"Transaction Log", "outBox");

			moneyMadeBox.Font = new Font("Calibri", 14);
			outBox.Multiline = true;
			outBox.AcceptsTab = true;
			outBox.WordWrap = true;
			outBox.ReadOnly = true;
			outBox.DetectUrls = true;
			// outBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			RunBot(0);

        } // end AlcoveShopBot

		public void buildMenuBar (){
			this.Menu = new MainMenu();
			
			MenuItem item = new MenuItem("File");
			this.Menu.MenuItems.Add(item);
				item.MenuItems.Add("Open log folder", new EventHandler(Open_Log_Folder)); 
			
			item = new MenuItem("Edit");
			this.Menu.MenuItems.Add(item);
				item.MenuItems.Add("Shop name", new EventHandler(Edit_Store_Name));
				item.MenuItems.Add("Shop coordinates", new EventHandler(Edit_Store_Coords)); 
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
//====================--------------------      Main     --------------------====================//
//////////////////////====================--------------------====================////////////////////
//////////////////////////////////////////====================////////////////////////////////////////
		public void RunBot (int setzero) {
			moneyMadeBox.Text = MoneyMade(0,false,true);
			List<StockItem> ShopData = GetShopData("");
			ShopData = ShopData.Where(s => s.Event.Contains("Empty")).ToList();
			ShopData =  ShopData.Where(s => !OldData.Any(o => o.Timestamp.Contains(s.Timestamp))).ToList();
			//foreach(StockItem item in OldData) ShopData.Remove(item);
			//ShopData = ShopData.Except(OldData).ToList();
			if (ShopData.Any() == true) {
			// outBox.Text = "ShopData Before:" + serializer.Serialize(ShopData) + Environment.NewLine + outBox.Text;
			// outBox.Text = "OldData Before:" + serializer.Serialize(OldData) + Environment.NewLine + outBox.Text;
				foreach (StockItem item in ShopData) {
					string out_msg = "Empty shop: Player `" + item.PlayerName + "` has purchased the last "+ item.StockQty + " " + item.ItemName+"!";
					outBox.Text = "[" + item.Timestamp + "] " +  out_msg + Environment.NewLine + outBox.Text;
					//SendDiscordMessage(out_msg);
				};
			OldData.AddRange(ShopData);
			// outBox.Text = "ShopData After:" + serializer.Serialize(ShopData) + Environment.NewLine + outBox.Text;
			// outBox.Text = "OldData After:" + serializer.Serialize(OldData) + Environment.NewLine + outBox.Text;
			}
		}

		public List<StockItem> GetShopData (string Logfile) {
			if (Logfile == "") {
				Logfile = LatestLog;
			}
			List<StockItem> out_var = new List<StockItem>();
			List<string> Data = new List<string>();
			string fiileString = null;
			StockItem stockitem = new StockItem();
			int n = 0;
			
			try {
					FileStream logFileStream = new FileStream(Logfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					StreamReader logFileReader = new StreamReader(logFileStream);
					while (!logFileReader.EndOfStream) {
						fiileString = logFileReader.ReadLine();
						//outBox.Text = fiileString + Environment.NewLine + outBox.Text;
						string[] fileContents = fiileString.Replace("] [Render thread/INFO]: [System] [CHAT] ",",").Split('\n');
						Data.AddRange(fileContents.Where(d => d.Contains("SHOPS ▶ ")).ToList());
						Data = Data.Where(d => !d.Contains("Enter all in chat")).ToList();
						Data = Data.Where(d => !d.Contains("out of space")).ToList();
						Data = Data.Where(d => !d.Contains("how many you wish")).ToList();
					}
				//outBox.Text = "FileStream Success."+ Logfile + " count: " + Data.Count + Environment.NewLine + outBox.Text;
					logFileReader.Close();
					logFileStream.Close();

			} catch (Exception FileStreamError) {
				outBox.Text = "FileStreamError: " + FileStreamError.Message + Environment.NewLine + outBox.Text;
				try {
			string[] fileContents = GetContent(Logfile, true).Replace("] [Render thread/INFO]: [System] [CHAT] ",",").Split('\n');
			Data.AddRange(fileContents.Where(d => d.Contains("SHOPS ▶ ")).ToList());
			Data = Data.Where(d => !d.Contains("Enter all in chat")).ToList();
			Data = Data.Where(d => !d.Contains("out of space")).ToList();
			Data = Data.Where(d => !d.Contains("how many you wish")).ToList();
				//outBox.Text = "GetContent Success."+ Logfile + " count: " + Data.Count + Environment.NewLine + outBox.Text;
				} catch (Exception GetContentError) {
					outBox.Text = "GetContentError: " + GetContentError.Message + Environment.NewLine + outBox.Text;
					fiileString = FileStreamError.Message + "; " + GetContentError.Message;
				}
			}


			//Data = Data | where {_ -notmatch ""}
			foreach (string Item in Data) {
				n++;
				stockitem = new StockItem();
				//Use all 3 coords because some places have 2 buy shops stacked at the same X and Z. 
				if (Item.Contains("has run out of")){
						string[] SplitItem = Item.Replace("[","").Replace("] Your shop at",",").Replace(" has run out of",",").Replace("!","").Replace(", ",",").Split(',');
						//06:07:40 ,15226, 63, 20487, Cocoa Beans
						stockitem.Timestamp = SplitItem[0];
						stockitem.XLoc = Convert.ToInt32(SplitItem[1].Split(' ').Last());
						stockitem.YLoc = Convert.ToInt32(SplitItem[2]);
						stockitem.ZLoc = Convert.ToInt32(SplitItem[3]);
						stockitem.ItemName = SplitItem[4];
						stockitem.Event = "Empty";
						// outBox.Text = stockitem.Timestamp + " - " + stockitem.Event + " - " + stockitem.ItemName + " - " + Environment.NewLine + outBox.Text;
						//outBox.Text = (Data[Data.IndexOf(Item)-1].Split(' '))[4] + Environment.NewLine + outBox.Text;
						stockitem.StockQty = Convert.ToInt32((Data[Data.IndexOf(Item)-1].Split(' '))[4]);
						stockitem.PlayerName = (Data[Data.IndexOf(Item)-1].Split(' '))[2];
		            } else if (Item.Contains("is now full")){
						string[] SplitItem = Item.Replace("[","").Replace("] Your shop at",",").Replace(" is now full.",", ").Replace("!","").Replace(", ",",").Split(',');
						//05:33:18, 15274, 66, 20463,
						stockitem.Timestamp = SplitItem[0];
						stockitem.XLoc = Convert.ToInt32(SplitItem[1].Split(' ').Last());
						stockitem.YLoc = Convert.ToInt32(SplitItem[2]);
						stockitem.ZLoc = Convert.ToInt32(SplitItem[3]);
						//stockitem.ItemName = SplitItem[4];
						stockitem.Event = "Full";
						//stockitem.StockQty = Convert.ToInt32((Data[Data.IndexOf(Item)-1].Split(' '))[3]);
						//stockitem.PlayerName = (Data[Data.IndexOf(Item)-1].Split(' '))[1];
		            } else if (Item.Contains("to your shop")){
						string[] SplitItem = Item.Replace("[","").Replace("]",",").Replace(" sold ",", ").Replace(" to your shop {3}.",", ").Replace("!","").Split(',');
						 //05:40:12, kota490, 1728 Sea Lantern,
						stockitem.Timestamp = SplitItem[0];
						stockitem.PlayerName = SplitItem[1];
						//stockitem.XLoc = SplitItem[1];
						//stockitem.YLoc = SplitItem[2];
						//stockitem.ZLoc = SplitItem[3];
						// string[] SplitItemArray = SplitItem[2].Split(' ').Skip(1).Take(9).ToArray();
						// stockitem.ItemName = String.Join(" ",SplitItemArray);
						stockitem.Event = "Sell";
						//stockitem.StockQty = Convert.ToInt32((Data[Data.IndexOf(Item)-1].Split(' '))[3]);
						//stockitem.PlayerName = (Data[Data.IndexOf(Item)-1].Split(' '))[1];
		            } else if (Item.Contains("from your shop")){
						string[] SplitItem = Item.Replace("[","").Replace("]",",").Replace(" purchased ",", ").Replace(" from your shop and you earned ",", ").Replace("(","").Split(',');
						//05:47:12 _Blackjack29313, 2 Grindstone, 9.50 0.50 in taxes).
						stockitem.Timestamp = SplitItem[0];
						stockitem.PlayerName = SplitItem[1];
						//stockitem.XLoc = SplitItem[1];
						//stockitem.YLoc = SplitItem[2];
						//stockitem.ZLoc = SplitItem[3];
						// string[] SplitItemArray = SplitItem[2].Split(' ').Skip(1).Take(9).ToArray();
						// stockitem.ItemName = String.Join(" ",SplitItemArray);
						stockitem.Event = "Buy";
						//stockitem.StockQty = Convert.ToInt32((Data[Data.IndexOf(Item)-1].Split(' '))[3]);
						//stockitem.PlayerName = (Data[Data.IndexOf(Item)-1].Split(' '))[1];
					}
				out_var.Add(stockitem);
			}//end foreach
		return out_var;
		}
		
/*
			//string[] fileContents = Encoding.UTF8.GetString(System.IO.File.ReadAllBytes(LatestLog)).Replace("] [Render thread/INFO]: [System] [CHAT] ",",").Split('\n');
			//Working w locks-----> string[] fileContents = GetContent(LatestLog).Replace("] [Render thread/INFO]: [System] [CHAT] ",",").Split('\n');
			//string[] fileContents = GetContent(LatestLog).Replace("] [Render thread/INFO]: [System] [CHAT] ","RemoveTimeSpecialTokenYouShouldNeverSeeThis]\n").Split('\n');
		//List<string> DataArray = new []{ Data };// select-string "SHOPS ▶ " | Where {_ -notmatch "Enter all in chat"}).Replace("\[Render thread/INFO\]: \[System\] \[CHAT\] SHOPS ▶ ","")
			// string EmptyText = "*has run out of*";//[06:07:40] Your shop at 15226, 63, 20487 has run out of Cocoa Beans!
			//[07:12:51] [Render thread/INFO]: [System] [CHAT] SHOPS ▶ This shop has run out of space! Contact the shop owner or staff to get it emptied.
			// string FullText = "*is now full*";//[05:33:18] Your shop at 15274, 66, 20463 is now full.
			// string SellText = "*to your shop*";//[05:40:12] kota490 sold 1728 Sea Lantern to your shop {3}.
			// string BuyText = "*from your shop*";//[05:47:12] _Blackjack29313 purchased 2 Grindstone from your shop and you earned 9.50 (0.50 in taxes).
			//dynamic[] NameData = FromCsv(GetContent(DataFile));
			//Data = Data.Where(d => !d.Contains("RemoveTimeSpecialTokenYouShouldNeverSeeThis")).ToList();


						// outBox.Text = stockitem.Event + " - " + Environment.NewLine + outBox.Text;
				//var NewName = NameData.Where(x => x["XLoc"] = stockitem.XLoc).Where(x => x["YLoc"] = stockitem.YLoc).Where(x => x["ZLoc"] = stockitem.ZLoc);
				//if (NewName != null) {
					//stockitem.ItemName = NewName.ToString();//["ItemName"]
				//}
*/
		//Reports
		public string[] DecompressDailyFiles (int day = 0) {
			  var now = DateTime.Now;
			if (day == 0) {
				day = Convert.ToInt32(now.AddDays(-1).ToString("dd"));
			}
			string date = new DateTime(now.Year, now.Month, day).ToString("yyyy-MM-dd");
			string[] UnzipFiles = Directory.GetFileSystemEntries(logFolder, date + "*.log.gz", SearchOption.AllDirectories);
			foreach (string Filename in UnzipFiles) {
				DeGZip(Filename);
			}
			string[] inputfiles = Directory.GetFileSystemEntries(logFolder, date + "*.log", SearchOption.AllDirectories);
			return inputfiles;
		}
		
		public List<StockItem> DailyData (int day = 0) {
			  var now = DateTime.Now;
			if (day == 0) {
				day = Convert.ToInt32(now.AddDays(-1).ToString("dd"));
			}
			List<StockItem> Data = new List<StockItem>();
			string[] inputfiles = DecompressDailyFiles(day);
			GetShopData(LatestLog); //Reading the log seems to nudge the filesystem into writing the gzip data.
			foreach (string Filename in inputfiles) {
			//outBox.Text = "Filename: " + Filename + Environment.NewLine + outBox.Text;
			var data_inter = GetShopData(Filename);
			//outBox.Text = "Inter count: " + data_inter.Count + Environment.NewLine + outBox.Text;
			Data.AddRange(data_inter);
			//outBox.Text = "Data count: " + Data.Count + Environment.NewLine + outBox.Text;
			}
			return Data;
		}

		public string MoneyMade (int day = 0, bool Format = false, bool Today = false) {
			string sum_string = null;
			  var now = DateTime.Now;
			int Sum = 0;
			string Data = null;
			
			if (Today) {
				day = now.Day;
				Data  += OldGetContent(LatestLog);
				
			} else {
				if (day == 0) {
					day = Convert.ToInt32(now.AddDays(-1).ToString("dd"));
				} 
				string[] inputfiles = DecompressDailyFiles(day);
				GetShopData(LatestLog); //Reading the log seems to nudge the filesystem into writing the gzip data.
				
				//outBox.Text = "inputfiles: " + inputfiles.Count() + Environment.NewLine + outBox.Text;
				//outBox.Text = "inputfiles: " + serializer.Serialize(inputfiles) + Environment.NewLine + outBox.Text;
				foreach (string Filename in inputfiles) {
					Data  += OldGetContent(Filename);
				}
			}
			string[] split_data = Data.Split('\n');
			split_data = split_data.Where(s => s.Contains("and you earned")).ToArray();
			foreach (string Datum in split_data) {
				string split = Datum.Split('$')[1].Replace(" (","");
				//outBox.Text = split + Environment.NewLine + outBox.Text;

				int number;
				int.TryParse(split, out number);
				Sum += number;
			}
			sum_string = Sum.ToString("C2");
			string date = new DateTime(now.Year, now.Month, day).ToString("yyyy-MM-dd");
			if (Format) {
				sum_string = date + " Alcove made " + sum_string;
			}
				return sum_string;
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
					Sold += (_.stockqty.Replace("-","")
				} catch {}
			}
			
			Purchased = 0
			Data | where {_.event -match Values.Sell}  | %{
				try {
					Purchased += (_.stockqty.Replace("-","")
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
			//OOS = (data.StockQty | where {_EVent -match Values.Empty}).count
			
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
		
		public static DialogResult drawInputDialog(ref string input) {
			System.Drawing.Size size = new System.Drawing.Size(200, 70);
			Form inputBox = new Form();

			inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			inputBox.ClientSize = size;
			inputBox.Text = "Name";

			System.Windows.Forms.TextBox textBox = new TextBox();
			textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
			textBox.Location = new System.Drawing.Point(5, 5);
			textBox.Text = input;
			inputBox.Controls.Add(textBox);

			Button okButton = new Button();
			okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			okButton.Name = "okButton";
			okButton.Size = new System.Drawing.Size(75, 23);
			okButton.Text = "&OK";
			okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
			inputBox.Controls.Add(okButton);

			Button cancelButton = new Button();
			cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new System.Drawing.Size(75, 23);
			cancelButton.Text = "&Cancel";
			cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
			inputBox.Controls.Add(cancelButton);

			inputBox.AcceptButton = okButton;
			inputBox.CancelButton = cancelButton; 

			DialogResult result = inputBox.ShowDialog();
			input = textBox.Text;
			return result;
		}






//////////////////////////////////////////====================////////////////////////////////////////
//////////////////////====================--------------------====================////////////////////
//===================--------------------   Utility Functions  --------------------====================
//////////////////////====================--------------------====================////////////////////
//////////////////////////////////////////====================////////////////////////////////////////
/*Powershell functional equivalency imperatives
		Get-Clipboard = Clipboard.GetText();
		Get-Date = Timestamp.Now.ToString("M/d/yyyy");
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
		
		Diff
		var inShopDataButNotInOldData = ShopData.Except(OldData);
		var inOldDataButNotInShopData = OldData.Except(ShopData);
		
*/
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
		//JSON
		public dynamic FromJson(string string_input) {
			dynamic dynamic_output = new System.Dynamic.ExpandoObject();
			dynamic_output = serializer.Deserialize<dynamic>(string_input);
			return dynamic_output;
		}

		public string ToJson(dynamic dynamic_input) {
			string string_out;
			string_out = serializer.Serialize(dynamic_input);
			return string_out;
		}
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
		//Non-locking alternative: System.IO.File.ReadAllBytes(Filename);
		public string GetContent(string Filename, bool NoErrorMessage = false) {
			string fiileString = null;
			outBox.Text = "fiileString Start" +  Environment.NewLine + outBox.Text;
			
			FileStream logFileStream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			StreamReader logFileReader = new StreamReader(logFileStream);
			while (!logFileReader.EndOfStream) {
				outBox.Text = "fiileString While" +  Environment.NewLine + outBox.Text;
				fiileString = logFileReader.ReadLine();
				outBox.Text = fiileString.Length + Environment.NewLine + outBox.Text;
			}
			outBox.Text = "FileStream Success."+ fiileString.Length + Environment.NewLine + outBox.Text;
			logFileReader.Close();
			logFileStream.Close();
			return fiileString;
		}
		public string OldGetContent(string Filename, bool NoErrorMessage = false) {
			string string_out = "";
				// Open the text file using a stream reader.
				using (var sr = new StreamReader(Filename)) {
					// Read the stream as a string, and write the string to the console.
					string_out = sr.ReadToEnd();
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
		//Discord
		public void SendDiscordMessage (string content) {
			string payload = "{\"content\": \"" + content + "\"}";
			InvokeWebRequest(webHook, WebRequestMethods.Http.Post, payload,false,true);
		}





//////////////////////////////////////////====================////////////////////////////////////////
//////////////////////====================--------------------====================////////////////////
//===================--------------------    Event Handlers   --------------------====================
//////////////////////====================--------------------====================////////////////////
//////////////////////////////////////////====================////////////////////////////////////////
		//timer
		private void timer_everysecond(object sender, EventArgs e) {
			RunBot(0);
		}
		//ui
        public void runButton_Click(object sender, EventArgs e) {
			string action = "Run";
			outBox.Text = action + Environment.NewLine + outBox.Text;
			timer.Start();
        }// end runButton_Click

        public void stopButton_Click(object sender, EventArgs e) {
			string action = "Stop";
			outBox.Text = action + Environment.NewLine + outBox.Text;
			timer.Stop();
        }// end stopButton_Click

        public void sendButton_Click(object sender, EventArgs e) {
			//Season a = Season.Autumn;
			//outBox.Text = a + Environment.NewLine + outBox.Text;
			// string[] filelist = GetShopData(0);
			// foreach (string file in filelist) {
				// outBox.Text = file + Environment.NewLine + outBox.Text;
			// }
			// List<StockItem> ShopData = DailyData (0);
			// ShopData = ShopData.Where(s => s.Event.Contains("Empty")).ToList();
			// outBox.Text = "ShopData count: " + ShopData.Count + Environment.NewLine + outBox.Text;
			// foreach (StockItem item in ShopData) {
				// string out_msg = "Demo: Player `" + item.PlayerName + "` has purchased the last "+ item.StockQty + " " + item.ItemName+"!";
				// outBox.Text = "[" + item.Timestamp + "] " +  out_msg + Environment.NewLine + outBox.Text;
			// };
			
			outBox.Text = MoneyMade(0) + Environment.NewLine + outBox.Text;
			
        }// end stopButton_Click

		public void OnResize(object sender, System.EventArgs e) {
		}
		//Menu
		//File
		public void Open_Log_Folder(object sender, EventArgs e) {
			Process.Start(logFolder);
		}// end Open_Log_Folder

		public void Edit_Store_Name(object sender, EventArgs e) {
			string input= StoreName;
			DialogResult result = drawInputDialog(ref input);
			switch (result) {
				case DialogResult.OK:
					StoreName = input;
					appTitle = StoreName + " Shop Bot - Build ";
					this.Text = appTitle + build;
					break;
			}
		}// end Edit_Store_Name

		public void Edit_Store_Coords(object sender, EventArgs e) {
			string input="Edit_Store_Coords";
			DialogResult result = drawInputDialog(ref input);
			switch (result) {
				case DialogResult.OK:
					outBox.Text = input + Environment.NewLine + outBox.Text;
					break;
			}
		}// end Edit_Store_Coords

		public void Edit_Webhook(object sender, EventArgs e) {
			string input="Edit_Webhook";
			DialogResult result = drawInputDialog(ref input);
			switch (result) {
				case DialogResult.OK:
					outBox.Text = input + Environment.NewLine + outBox.Text;
					break;
			}
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
	
	public class StockItem  {
	  public string ItemName;
	  public int XLoc;
	  public int YLoc;
	  public int ZLoc;
	  public int StockQty;
	  public string Event;
	  public string Timestamp;
	  public string PlayerName;
	}
}// end ShopBotNamespace

