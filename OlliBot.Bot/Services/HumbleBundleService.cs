using Microsoft.Playwright;
using OlliBot.Bot.Modules;
using OlliBot.Domain.Entities;

namespace OlliBot.Bot.Services;
public class HumbleBundleService
{
    public async Task<List<HumbleBundle>> GetAllBundleDetailsAsync()
    {
        IPlaywright pw = await Playwright.CreateAsync();
        IBrowser browser = await pw.Chromium.LaunchAsync();
        IBrowserContext context = await browser.NewContextAsync();
        IPage gamesPage = await context.NewPageAsync();

        await gamesPage.GotoAsync("https://www.humblebundle.com/games");

        ILocator bundles = gamesPage.Locator(".full-tile-view.one-third.bundle");
        int bundleCount = await bundles.CountAsync();

        var humbleBundles = new List<HumbleBundle>();

        for (int i = 0; i < bundleCount; i++)
        {
            var humbleBundle = new HumbleBundle();
            ILocator bundle = bundles.Nth(i);
            string bundleName = await bundle.Locator(".name").InnerHTMLAsync();
            humbleBundle.Name = bundleName;

            string? href = await bundle.GetAttributeAsync("href");
            try
            {
                await GetBundleDetails(context, href, humbleBundle);
            }
            catch (Exception ex)
            {

            }
        }
        await context.CloseAsync();
        return humbleBundles;
    }

    public async Task<HumbleBundle> GetBundleDetails(IBrowserContext context, string href, HumbleBundle bundle)
    {
        IPage page = await context.NewPageAsync();
        await page.GotoAsync("https://www.humblebundle.com" + href);

        ILocator tierFilters = page.Locator(".js-tier-filter.chip");
        int tierCount = Math.Max(1, await tierFilters.CountAsync());

        for (int i = 0; i < tierCount; i++)
        {
            var bundleTier = new HumbleBundleTier();
            bundleTier.Tier = i;
            ILocator tierFilter = tierFilters.Nth(i);
            if (await tierFilter.IsVisibleAsync())
                await tierFilter.ClickAsync();

            ILocator bundleItems = page.Locator(".tier-item-view");
            //var tierHeader = page.Locator(".tier-header.js-tier-header");
            //await Task.Delay(160);

            if (i != 0)
            {
                await page.WaitForFunctionAsync("() => !document.querySelector('.tier-item-view.flipping')");
            }

            string header = await page.Locator(".tier-header.js-tier-header").First.InnerTextAsync();

            /*
            var bundleItems = page.Locator(".tier-item-view");
            var tierHeader = page.Locator(".tier-header.js-tier-header");
            */
            //Console.WriteLine(await tierHeader.InnerTextAsync());

            if (i != 0)
                Console.WriteLine();
            Console.WriteLine(new string('/', header.Length));
            Console.WriteLine(header);
            Console.WriteLine(new string('/', header.Length));
            Console.WriteLine();

            int itemCount = await bundleItems.CountAsync();

            for (int j = 0; j < itemCount; j++)
            {
                string title = await bundleItems.Nth(j).Locator(".item-title").InnerTextAsync();
                ILocator extraInfo = bundleItems.Nth(j).Locator(".extra-info.fine-print");
                if (await extraInfo.CountAsync() > 0)
                {
                    Console.WriteLine($"{title} ({await extraInfo.InnerTextAsync()})");
                }
                else
                    Console.WriteLine($"{title}");
            }
        }
        await page.CloseAsync();
        return bundle;
    }
}
