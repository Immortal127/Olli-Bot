using Microsoft.Playwright;
using OlliBot.Application.HumbleBundle;
using OlliBot.Application.HumbleBundle.Models;
using OlliBot.Domain.Enums;

namespace OlliBot.Infrastructure.HumbleBundle;
public class HumbleBundleScanner : IHumbleBundleScanner
{
    public async Task<List<ScannedHumbleBundle>> ScanAsync(HumbleBundleType bundleType, CancellationToken ct)
    {
        IPlaywright pw = await Playwright.CreateAsync();
        IBrowser browser = await pw.Chromium.LaunchAsync();
        IBrowserContext context = await browser.NewContextAsync();
        IPage page = await context.NewPageAsync();

        string type = bundleType switch
        {
            HumbleBundleType.Game => "games",
            HumbleBundleType.Book => "books",
            HumbleBundleType.Software => "software",
            _ => "games"
        };

        await page.GotoAsync("https://www.humblebundle.com/" + type);

        ILocator bundles = page.Locator(".full-tile-view.one-third.bundle");
        int bundleCount = await bundles.CountAsync();

        var humbleBundles = new List<ScannedHumbleBundle>();

        for (int i = 0; i < bundleCount; i++)
        {
            ILocator bundle = bundles.Nth(i);
            string bundleName = await bundle.Locator(".name").InnerHTMLAsync();
            // Will need to adjust this when considering other types of bundles i.e books, software
            var humbleBundle = new ScannedHumbleBundle
            {
                Name = bundleName,
                BundleType = bundleType,
            };

            string? href = await bundle.GetAttributeAsync("href");
            try
            {
                await GetBundleDetails(context, href, humbleBundle);
                humbleBundles.Add(humbleBundle);
            }
            catch (Exception)
            {

            }
        }
        await context.CloseAsync();
        return humbleBundles;
    }

    public async Task<ScannedHumbleBundle> GetBundleDetails(IBrowserContext context, string href, ScannedHumbleBundle bundle)
    {
        IPage page = await context.NewPageAsync();
        await page.GotoAsync("https://www.humblebundle.com" + href);

        ILocator tierFilters = page.Locator(".js-tier-filter.chip");
        int tierCount = Math.Max(1, await tierFilters.CountAsync());

        for (int tier = 0; tier < tierCount; tier++)
        {
            var bundleTier = new ScannedHumbleBundleTier
            {
                Tier = tier,
            };
            ILocator tierFilter = tierFilters.Nth(tier);
            if (await tierFilter.IsVisibleAsync())
                await tierFilter.ClickAsync();

            ILocator bundleItems = page.Locator(".tier-item-view");
            //var tierHeader = page.Locator(".tier-header.js-tier-header");
            //await Task.Delay(160);

            if (tier != 0)
            {
                await page.WaitForFunctionAsync("() => !document.querySelector('.tier-item-view.flipping')");
            }

            string priceHeader = await page.Locator(".tier-header.js-tier-header").First.InnerTextAsync();

            /*
            var bundleItems = page.Locator(".tier-item-view");
            var tierHeader = page.Locator(".tier-header.js-tier-header");
            */
            //Console.WriteLine(await tierHeader.InnerTextAsync());

            //if (i != 0)
            //    Console.WriteLine();
            //Console.WriteLine(new string('/', header.Length));
            //Console.WriteLine(header);
            //Console.WriteLine(new string('/', header.Length));
            //Console.WriteLine();

            int itemCount = await bundleItems.CountAsync();

            for (int item = 0; item < itemCount; item++)
            {
                string title = await bundleItems.Nth(item).Locator(".item-title").InnerTextAsync();
                string extraInfo = await bundleItems.Nth(item).Locator(".extra-info.fine-print").InnerTextAsync();

                // Might need to look into what happens if there is no extra info when calling the InnerTextAsync method
                var bundleItem = new ScannedHumbleBundleItem
                {
                    ItemName = title,
                    //ExtraInfo = await extraInfo.InnerTextAsync(),
                    ExtraInfo = extraInfo
                };

                //if (await extraInfo.CountAsync() > 0)
                //{
                //    Console.WriteLine($"{title} ({await extraInfo.InnerTextAsync()})");
                //}
                //else
                //    Console.WriteLine($"{title}");

                bundleTier.HumbleBundleItems.Add(bundleItem);
            }
            bundle.BundleTiers.Add(bundleTier);
        }
        await page.CloseAsync();
        return bundle;
    }
}
