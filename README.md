# AlcoveShopBot
Generates out-of-stock alerts and financial reports from QuickShop buy/sell comments in Minecraft chat logs. Made for The Alcove player-run store on the ChillSMP Minecraft server. Not affiliated with any of those. 

![Screenshot of Alcove Shop Bot](https://github.com/Gilgamech/AlcoveShopBot/blob/main/AlcoveShopBot.png)

### Install steps:

- Copy to permanent location. Wherever you run it from, it has to stay there, or you'll have to put the webhook in again.
- Double-click. 
- Windows will pop up a big blue box. Click the "More Info" link near the top. 

![Windows Protected Your PC screen with More Info link](https://github.com/Gilgamech/AlcoveShopBot/blob/main/MoreInfo.png)

- Then, click the "Run Anyway" button near the bottom. 

![Windows Protected Your PC screen with Run Anyway button](https://github.com/Gilgamech/AlcoveShopBot/blob/main/RunAnyway.png)

- App will open. 
- Click "Run" and the bot should display all old out of stocks in your log. 
- App will continue to log events. 
- Adding a webhook for a Discord channel will also send these messages to the Discord channel. 
  - Click the `Edit` menu, then `Edit Webhook`. 
  - Go to Discord:
    - Hold the channel name or click the gear to "Edit Channel"
    - Find and select the "Integrations" section
    - Then the "Webhooks" section. 
	- Add a new webhook, and name it. 
	- Please refer to [Discord's Intro to Webhooks](https://support.discord.com/hc/en-us/articles/228383668-Intro-to-Webhooks) for more info.
  - Copy the webhook URL from Discord. Go back to the Shop Bot app, and paste it in the `Edit Webhook` box. 
    - Be careful with the webhook URL - don't share it or let others see it. Or someone could send David Hasselhoff memes (or worse) to your shop channel!

It might freeze sometimes, if your Minecraft log gets really big (more than 1 million lines), so wait bit. If it freezes for a really long time, click "Stop" a few times and wait for a while, to see if it stops and was just reading a long file. If it never stops, open an [Issue](https://github.com/Gilgamech/AlcoveShopBot/issues).

