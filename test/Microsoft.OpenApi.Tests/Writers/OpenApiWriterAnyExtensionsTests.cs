﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    public class OpenApiWriterAnyExtensionsTests
    {
        [Fact]
        public void WriteOpenApiNullAsJsonWorks()
        {
            // Arrange
            var nullValue = new OpenApiNull();

            var json = WriteAsJson(nullValue);

            // Assert
            Assert.Equal("null", json);
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(0)]
        [InlineData(42)]
        public void WriteOpenApiIntergerAsJsonWorks(int input)
        {
            // Arrange
            var intValue = new OpenApiInteger(input);

            var json = WriteAsJson(intValue);

            // Assert
            Assert.Equal(input.ToString(), json);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WriteOpenApiBooleanAsJsonWorks(bool input)
        {
            // Arrange
            var boolValue = new OpenApiBoolean(input);

            var json = WriteAsJson(boolValue);

            // Assert
            Assert.Equal(input.ToString().ToLower(), json);
        }

        [Fact]
        public void WriteOpenApiObjectAsJsonWorks()
        {
            // Arrange
            var openApiObject = new OpenApiObject
            {
                {"stringProp", new OpenApiString("stringValue1")},
                {"objProp", new OpenApiObject()},
                {
                    "arrayProp",
                    new OpenApiArray
                    {
                        new OpenApiBoolean(false)
                    }
                }
            };

            var actualJson = WriteAsJson(openApiObject);

            // Assert

            var expectedJson = @"{
  ""stringProp"": ""stringValue1"",
  ""objProp"": { },
  ""arrayProp"": [
    false
  ]
}";
            expectedJson = expectedJson.MakeLineBreaksEnvironmentNeutral();

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void WriteOpenApiArrayAsJsonWorks()
        {
            // Arrange
            var openApiObject = new OpenApiObject
            {
                {"stringProp", new OpenApiString("stringValue1")},
                {"objProp", new OpenApiObject()},
                {
                    "arrayProp",
                    new OpenApiArray
                    {
                        new OpenApiBoolean(false)
                    }
                }
            };

            var array = new OpenApiArray
            {
                new OpenApiBoolean(false),
                openApiObject,
                new OpenApiString("stringValue2")
            };

            var actualJson = WriteAsJson(array);

            // Assert

            var expectedJson = @"[
  false,
  {
    ""stringProp"": ""stringValue1"",
    ""objProp"": { },
    ""arrayProp"": [
      false
    ]
  },
  ""stringValue2""
]";

            expectedJson = expectedJson.MakeLineBreaksEnvironmentNeutral();

            Assert.Equal(expectedJson, actualJson);
        }

        private static string WriteAsJson(IOpenApiAny any)
        {
            // Arrange (continued)
            var stream = new MemoryStream();
            IOpenApiWriter writer = new OpenApiJsonWriter(new StreamWriter(stream));

            writer.WriteAny(any);
            writer.Flush();
            stream.Position = 0;

            // Act
            var value = new StreamReader(stream).ReadToEnd();

            if (any.AnyKind == AnyTypeKind.Primitive || any.AnyKind == AnyTypeKind.Null)
            {
                return value;
            }

            return value.MakeLineBreaksEnvironmentNeutral();
        }
    }
}