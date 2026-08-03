using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.DataAccess.Helpers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mil.Paperwork.Tests.History
{
    /// <summary>
    /// Guards the polymorphic serialization contract of <see cref="ReportSnapshotBase"/>.
    /// Adding a new report snapshot without a matching [JsonDerivedType] entry makes "save draft"
    /// throw at runtime ("Runtime type ... is not supported by polymorphic type ..."), which no
    /// mapper-level test catches.
    /// </summary>
    public class ReportSnapshotPolymorphismTests
    {
        private static List<Type> GetConcreteSnapshotTypes()
        {
            var types = typeof(ReportSnapshotBase).Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ReportSnapshotBase)) && !t.IsAbstract)
                .OrderBy(t => t.Name)
                .ToList();

            return types;
        }

        private static List<JsonDerivedTypeAttribute> GetDerivedTypeAttributes()
        {
            var attributes = typeof(ReportSnapshotBase)
                .GetCustomAttributes<JsonDerivedTypeAttribute>()
                .ToList();

            return attributes;
        }

        [Fact]
        public void EveryConcreteSnapshotType_IsRegisteredAsJsonDerivedType()
        {
            var registered = GetDerivedTypeAttributes().Select(a => a.DerivedType).ToHashSet();

            var missing = GetConcreteSnapshotTypes()
                .Where(t => !registered.Contains(t))
                .Select(t => t.Name)
                .ToList();

            Assert.True(
                missing.Count == 0,
                $"These ReportSnapshotBase subclasses are missing a [JsonDerivedType] entry on ReportSnapshotBase "
                    + $"and will throw when saved as a draft: {string.Join(", ", missing)}");
        }

        [Fact]
        public void TypeDiscriminators_AreUnique()
        {
            var discriminators = GetDerivedTypeAttributes()
                .Select(a => a.TypeDiscriminator?.ToString())
                .ToList();

            var duplicates = discriminators
                .GroupBy(d => d)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            Assert.True(duplicates.Count == 0, $"Duplicate type discriminators: {string.Join(", ", duplicates)}");
        }

        [Fact]
        public void EveryConcreteSnapshotType_RoundTripsThroughTheHistorySerializer()
        {
            foreach (var type in GetConcreteSnapshotTypes())
            {
                var instance = (ReportSnapshotBase)Activator.CreateInstance(type)!;

                var json = JsonSerializer.Serialize(instance, HistoryJsonOptions.Default);
                var restored = JsonSerializer.Deserialize<ReportSnapshotBase>(json, HistoryJsonOptions.Default);

                Assert.IsType(type, restored);
            }
        }
    }
}
