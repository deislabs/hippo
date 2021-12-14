using System;
using System.Collections.Generic;
using FluentValidation.Results;
using Hippo.Application.Common.Exceptions;
using Xunit;

namespace Hippo.Application.UnitTests.Common;

public class ValidationExceptionTests
{
    [Fact]
    public void DefaultConstructorCreatesAnEmptyErrorDictionary()
    {
        var actual = new ValidationException().Errors;

        Assert.Empty(actual.Keys);
    }

    [Fact]
    public void SingleValidationFailureCreatesASingleElementErrorDictionary()
    {
        var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Age", "must be over 18"),
            };

        var actual = new ValidationException(failures).Errors;

        Assert.Equal(new string[] { "Age" }, actual.Keys);
        Assert.Equal(new string[] { "must be over 18" }, actual["Age"]);
    }

    [Fact]
    public void MulitpleValidationFailureForMultiplePropertiesCreatesAMultipleElementErrorDictionaryEachWithMultipleValues()
    {
        var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Age", "must be 18 or older"),
                new ValidationFailure("Age", "must be 25 or younger"),
                new ValidationFailure("Password", "must contain at least 8 characters"),
                new ValidationFailure("Password", "must contain a digit"),
                new ValidationFailure("Password", "must contain upper case letter"),
                new ValidationFailure("Password", "must contain lower case letter"),
            };

        var actual = new ValidationException(failures).Errors;

        Assert.Equal(new string[] { "Age", "Password" }, actual.Keys);
        Assert.Equal(
            new string[]
            {
                "must be 18 or older",
                "must be 25 or younger"
            },
            actual["Age"]
        );
        Assert.Equal(
            new string[]
            {
                "must contain at least 8 characters",
                "must contain a digit",
                "must contain upper case letter",
                "must contain lower case letter"
            },
            actual["Password"]
        );
    }
}
