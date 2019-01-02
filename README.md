# RazorStripe

RazorStripe is an ASP.NET Core 2.2 Template that combines Razor Pages with Stripe integration.

## Run the Project
1. Download/fork the project and run the .sln
2. Add your own stripe keys in the [appsettings](appsettings.json)
3. Add your credentials in the [EmailSender](/Services/EmailSender.cs)
4. Re-save the [libman file](libman.json) and verify that the 5 folders have been downloaded into the [wwwroot](/wwwroot).
5. Run on IIS Express

## Custom Templates
There is also a .template.config folder so you can use dotnet new -i to create your own templates from this. More information [HERE](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates)

## Community
Contributions are welcome! Follow the community and contribute [HERE](https://discord.gg/6SAfBMc)

## License

See the [LICENSE](LICENSE.md) file for license rights and limitations (MIT).
