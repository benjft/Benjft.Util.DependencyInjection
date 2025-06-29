using System;
using Benjft.Util.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Benjft.Util.DependencyInjection.Tests.Extensions;

public class ServiceDescriptorWrapperTests
{
    private static ServiceDescriptorWrapper Wrap(ServiceDescriptor sd, int order) =>
        new ServiceDescriptorWrapper(sd, order);

    private class A {}
    private class B {}

    [Fact]
    public void CompareTo_SameReference_ReturnsZero()
    {
        var sd = ServiceDescriptor.Describe(typeof(A), typeof(A), ServiceLifetime.Transient);
        var w = Wrap(sd, 1);
        Assert.Equal(0, w.CompareTo(w));
    }

    [Fact]
    public void CompareTo_NullOther_ReturnsPositive()
    {
        var sd = ServiceDescriptor.Describe(typeof(A), typeof(A), ServiceLifetime.Transient);
        var w = Wrap(sd, 1);
        Assert.True(w.CompareTo(null) > 0);
    }

    [Fact]
    public void CompareTo_OrdersByOrder_FirstCriterion()
    {
        var sd = ServiceDescriptor.Describe(typeof(A), typeof(A), ServiceLifetime.Transient);
        var w1 = Wrap(sd, 0);
        var w2 = Wrap(sd, 1);
        Assert.True(w1.CompareTo(w2) < 0);
        Assert.True(w2.CompareTo(w1) > 0);
    }

    [Fact]
    public void CompareTo_TiesByOrder_ThenByServiceTypeFullName()
    {
        var sdA = ServiceDescriptor.Describe(typeof(A), typeof(A), ServiceLifetime.Transient);
        var sdB = ServiceDescriptor.Describe(typeof(B), typeof(B), ServiceLifetime.Transient);
        var wA = Wrap(sdA, 0);
        var wB = Wrap(sdB, 0);
        // FullName of A should be less than B lexicographically
        Assert.True(wA.CompareTo(wB) < 0 || wA.CompareTo(wB) == string.Compare(typeof(A).FullName, typeof(B).FullName, StringComparison.Ordinal));
    }

    [Fact]
    public void CompareTo_TiesByType_NonKeyedBeforeKeyed()
    {
        var sdUnkeyed = ServiceDescriptor.Describe(typeof(A), typeof(A), ServiceLifetime.Transient);
        var sdKeyed = ServiceDescriptor.DescribeKeyed(typeof(A), "k", typeof(A), ServiceLifetime.Transient);
        var wUnkeyed = Wrap(sdUnkeyed, 0);
        var wKeyed = Wrap(sdKeyed, 0);
        Assert.True(wUnkeyed.CompareTo(wKeyed) < 0);
        Assert.True(wKeyed.CompareTo(wUnkeyed) > 0);
    }

    [Fact]
    public void CompareTo_TiesByTypeAndKeyed_ComparesKeys_AndCatchesIncomparable()
    {
        var sdKeyed1 = ServiceDescriptor.DescribeKeyed(typeof(A), 5, typeof(A), ServiceLifetime.Transient);
        var sdKeyed2 = ServiceDescriptor.DescribeKeyed(typeof(A), 10, typeof(A), ServiceLifetime.Transient);
        var w1 = Wrap(sdKeyed1, 0);
        var w2 = Wrap(sdKeyed2, 0);
        Assert.True(w1.CompareTo(w2) < 0);

        // Now use incomparable keys (objects do not implement IComparable) to hit catch and return 0
        var sdObj1 = ServiceDescriptor.DescribeKeyed(typeof(A), new object(), typeof(A), ServiceLifetime.Transient);
        var sdObj2 = ServiceDescriptor.DescribeKeyed(typeof(A), new object(), typeof(A), ServiceLifetime.Transient);
        var wo1 = Wrap(sdObj1, 0);
        var wo2 = Wrap(sdObj2, 0);
        Assert.Equal(0, wo1.CompareTo(wo2));
    }

    [Fact]
    public void CompareTo_ObjectOverload_WrongType_Throws()
    {
        var sd = ServiceDescriptor.Describe(typeof(A), typeof(A), ServiceLifetime.Transient);
        var w = Wrap(sd, 1);
        Assert.Throws<ArgumentException>(() => { var _ = ((IComparable)w).CompareTo("not a wrapper"); });
    }

    [Fact]
    public void CompareTo_ObjectOverload_CorrectType_DoesNotThrow()
    {
        var sd = ServiceDescriptor.Describe(typeof(A), typeof(A), ServiceLifetime.Transient);
        var w = Wrap(sd, 1);
        var sd2 = ServiceDescriptor.Describe(typeof(B), typeof(B), ServiceLifetime.Transient);
        var w2 = Wrap(sd2, 1);
        var _ = ((IComparable)w).CompareTo(w2 as object);
    }

    [Fact]
    public void Deconstruct_ReturnsDescriptorAndOrder()
    {
        var sd = ServiceDescriptor.Describe(typeof(A), typeof(A), ServiceLifetime.Singleton);
        var w = Wrap(sd, 7);
        (ServiceDescriptor sdOut, int orderOut) = w;
        Assert.Same(sd, sdOut);
        Assert.Equal(7, orderOut);
    }
}
