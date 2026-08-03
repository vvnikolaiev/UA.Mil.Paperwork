using System.Text.Json.Serialization;

namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(InvoiceReportSnapshot), "invoice")]
    [JsonDerivedType(typeof(ResidualValueReportSnapshot), "residualValue")]
    [JsonDerivedType(typeof(InitialTechnicalStateReportSnapshot), "initialTechnicalState")]
    [JsonDerivedType(typeof(TechnicalStateReportSnapshot), "technicalState")]
    [JsonDerivedType(typeof(WriteOffPackageReportSnapshot), "writeOffPackage")]
    [JsonDerivedType(typeof(CommissioningActReportSnapshot), "commissioningAct")]
    [JsonDerivedType(typeof(ValuationReportSnapshot), "valuation")]
    [JsonDerivedType(typeof(DismantlingReportSnapshot), "dismantling")]
    [JsonDerivedType(typeof(Handover23ReportSnapshot), "handover23")]
    [JsonDerivedType(typeof(WriteOffOrderReportSnapshot), "writeOffOrder")]
    [JsonDerivedType(typeof(EASReportSnapshot), "eas")]
    public abstract class ReportSnapshotBase
    {
    }
}
