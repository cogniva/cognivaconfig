namespace Cogniva.Configuration;

public record LocalisedTextFact(ILocalisedText? LocalisedText) : IItemFact
{
    public string Label => LocalisedText.TextLabel;
}