using Nuna.Lib.TransactionHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.Helpers.DocNumberGenerator;

public class DocNumberGenerator: IDocNumberGenerator
{
    private readonly IDocTypeNumberFormatDal _numberFormatDal;
    private readonly IDocTypeNumberValueDal _numberValueDal;

    public DocNumberGenerator(IDocTypeNumberFormatDal numberFormatDal, IDocTypeNumberValueDal numberValueDal)
    {
        _numberFormatDal = numberFormatDal;
        _numberValueDal = numberValueDal;
    }

    public string Generate(IDocTypeKey docType, DateTime dateCreated)
    {
        var numberFormat = _numberFormatDal.GetData(docType);

        if (numberFormat is null)
            return string.Empty;

        var numberValue = numberFormat.ResetBy switch
        {
            ResetByEnum.Day => GetNumberDailyReset(numberFormat, dateCreated),
            ResetByEnum.Month => GetNumberMonthlyReset(numberFormat, dateCreated),
            ResetByEnum.Year => GetNumberYearlyReset(numberFormat, dateCreated),
            ResetByEnum.Continuous => GetNumberContinuous(numberFormat),
            _ => throw new ArgumentOutOfRangeException()
        };

        var docNumber = FormattingNumber(numberFormat.Format, numberValue, dateCreated);

        return docNumber;
    }

    public int GetNumberDailyReset(DocTypeNumberFormatModel numberFormat, DateTime dateCreated)
    {
        var listNumbering = _numberValueDal.ListData(numberFormat)?.ToList() ?? [];
        var latestNumbering = listNumbering
            .OrderByDescending(x => x.PeriodeTahun)
            .ThenByDescending(x => x.PeriodeBulan)
            .ThenByDescending(x => x.PeriodeHari)
            .FirstOrDefault();

        int numberValue;
        if (latestNumbering is null || dateCreated > new DateTime(latestNumbering.PeriodeTahun,
                latestNumbering.PeriodeBulan,
                latestNumbering.PeriodeHari))
        {
            var newNumbering = new DocTypeNumberValueModel
            {
                DocTypeId = numberFormat.DocTypeId,
                Value = 1,
                PeriodeHari = dateCreated.Day,
                PeriodeBulan = dateCreated.Month,
                PeriodeTahun = dateCreated.Year
            };
            numberValue = newNumbering.Value;
            newNumbering.Value++;
            listNumbering.Add(newNumbering);
        }
        else
        {
            numberValue = latestNumbering.Value;
            latestNumbering.Value++;
        }

        using var trans = TransHelper.NewScope();
        _numberValueDal.Delete(numberFormat);
        _numberValueDal.Insert(listNumbering);
        trans.Complete();

        return numberValue;
    }

    public int GetNumberMonthlyReset(DocTypeNumberFormatModel numberFormat, DateTime dateCreated)
    {
        var listNumbering = _numberValueDal.ListData(numberFormat)?.ToList() ?? [];
        var latestNumbering = listNumbering
            .OrderByDescending(x => x.PeriodeTahun)
            .ThenByDescending(x => x.PeriodeBulan)
            .FirstOrDefault();

        int numberValue;
        var dateCreatedMonthAndYear = new DateTime(dateCreated.Year, dateCreated.Month, 1);
        if (latestNumbering is null || dateCreatedMonthAndYear >
            new DateTime(latestNumbering.PeriodeTahun, latestNumbering.PeriodeBulan, 1))
        {
            var newNumbering = new DocTypeNumberValueModel
            {
                DocTypeId = numberFormat.DocTypeId,
                Value = 1,
                PeriodeHari = 1,
                PeriodeBulan = dateCreated.Month,
                PeriodeTahun = dateCreated.Year
            };
            numberValue = newNumbering.Value;
            newNumbering.Value++;
            listNumbering.Add(newNumbering);
        }
        else
        {
            numberValue = latestNumbering.Value;
            latestNumbering.Value++;
        }

        using var trans = TransHelper.NewScope();
        _numberValueDal.Delete(numberFormat);
        _numberValueDal.Insert(listNumbering);
        trans.Complete();

        return numberValue;
    }

    public int GetNumberYearlyReset(DocTypeNumberFormatModel numberFormat, DateTime dateCreated)
    {
        var listNumbering = _numberValueDal.ListData(numberFormat)?.ToList() ?? [];
        var latestNumbering = listNumbering
            .OrderByDescending(x => x.PeriodeTahun)
            .FirstOrDefault();

        int numberValue;
        if (latestNumbering is null || dateCreated.Year > latestNumbering.PeriodeTahun)
        {
            var newNumbering = new DocTypeNumberValueModel
            {
                DocTypeId = numberFormat.DocTypeId,
                Value = 1,
                PeriodeHari = 1,
                PeriodeBulan = 1,
                PeriodeTahun = dateCreated.Year
            };
            numberValue = newNumbering.Value;
            newNumbering.Value++;
            listNumbering.Add(newNumbering);
        }
        else
        {
            numberValue = latestNumbering.Value;
            latestNumbering.Value++;
        }

        using var trans = TransHelper.NewScope();
        _numberValueDal.Delete(numberFormat);
        _numberValueDal.Insert(listNumbering);
        trans.Complete();

        return numberValue;
    }

    public int GetNumberContinuous(DocTypeNumberFormatModel numberFormat)
    {
        var listNumbering = _numberValueDal.ListData(numberFormat)?.ToList() ?? [];
        var latestNumbering = listNumbering
            .OrderByDescending(x => x.PeriodeTahun)
            .FirstOrDefault();

        int numberValue;
        if (latestNumbering is null)
        {
            var newNumbering = new DocTypeNumberValueModel
            {
                DocTypeId = numberFormat.DocTypeId,
                Value = 1,
                PeriodeHari = 1,
                PeriodeBulan = 1,
                PeriodeTahun = 3000
            };
            numberValue = newNumbering.Value;
            newNumbering.Value++;
            listNumbering.Add(newNumbering);
        }
        else
        {
            numberValue = latestNumbering.Value;
            latestNumbering.Value++;
        }

        using var trans = TransHelper.NewScope();
        _numberValueDal.Delete(numberFormat);
        _numberValueDal.Insert(listNumbering);
        trans.Complete();

        return numberValue;
    }

    private string FormattingNumber(string numberFormat, int numberValue, DateTime dateCreated)
    {
        var extractedFormat = numberFormat.Split("/");
        for (var i = 0; i < extractedFormat.Length; i++)
        {
            extractedFormat[i] = extractedFormat[i] switch
            {
                "{NOURUT,1}" => $"{numberValue:D1}",
                "{NOURUT,2}" => $"{numberValue:D2}",
                "{NOURUT,3}" => $"{numberValue:D3}",
                "{ROMAWIM}" => GetMonthInRoman(dateCreated.Month),
                "{Y,2}" => $"{dateCreated:yy}",
                "{Y,4}" => $"{dateCreated:yyyy}",
                "{Y,2-NOURUT,1}" => $"{dateCreated:yy}-{numberValue:D1}",
                "{Y,2-NOURUT,2}" => $"{dateCreated:yy}-{numberValue:D2}",
                "{Y,2-NOURUT,3}" => $"{dateCreated:yy}-{numberValue:D3}",
                "{Y,4-NOURUT,1}" => $"{dateCreated:yyyy}-{numberValue:D1}",
                "{Y,4-NOURUT,2}" => $"{dateCreated:yyyy}-{numberValue:D2}",
                "{Y,4-NOURUT,3}" => $"{dateCreated:yyyy}-{numberValue:D3}",
                _ => extractedFormat[i]
            };
        }
        return extractedFormat.Join("/");
    }

    private string GetMonthInRoman(int month)
    {
        return month switch
        {
            1 => "I",
            2 => "II",
            3 => "III",
            4 => "IV",
            5 => "V",
            6 => "VI",
            7 => "VII",
            8 => "VIII",
            9 => "IX",
            10 => "X",
            11 => "XI",
            12 => "XII",
            _ => ""
        };
    }
}