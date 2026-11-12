using System;
using GreatCompany.Data.Models;
using GreatCompany.Data.Taxes;
using NUnit.Framework;

namespace GreatCompany.Test;

public class TaxCalculatorTest {
    [TestCase(2025, 12, 31, 20)]
    [TestCase(2026, 1, 1, 22)]
    public void RateChangesOnBoundary(int year, int month, int day, int expected) =>
        Assert.That(TaxCalculator.GetVatRatePercent(new DateTime(year, month, day)), Is.EqualTo(expected));

    [TestCase(120, 2025, 20)]
    [TestCase(122, 2026, 22)]
    [TestCase(-122, 2026, -22)]
    public void VatIsExtractedFromTotal(int total, int year, int expected) =>
        Assert.That(TaxCalculator.CalculateVat(TaxRegime.Vat, total, new DateTime(year, 1, 1)), Is.EqualTo(expected));

    [Test]
    public void MissingRegimeOrAmountProducesZero() {
        Assert.That(TaxCalculator.CalculateVat(null, 122m, new DateTime(2026, 1, 1)), Is.Zero);
        Assert.That(TaxCalculator.CalculateVat(TaxRegime.Vat, null, new DateTime(2026, 1, 1)), Is.Zero);
    }

    [TestCase(0, true)]
    [TestCase(23, true)]
    [TestCase(23.01, false)]
    public void VatValidationAllowsZeroAndTolerance(decimal vat, bool valid) =>
        Assert.That(TaxCalculator.CheckVat(TaxRegime.Vat, 122m, vat, new DateTime(2026, 1, 1)) == null, Is.EqualTo(valid));

    [Test]
    public void NonVatRegimeRejectsNonzeroVat() =>
        Assert.That(TaxCalculator.CheckVat(null, 122m, 1m, new DateTime(2026, 1, 1)), Is.Not.Null);
}
