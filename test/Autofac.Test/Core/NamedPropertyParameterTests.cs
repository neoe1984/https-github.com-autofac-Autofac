﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Reflection;
using Autofac.Core;

namespace Autofac.Test.Core
{
    public class NamedPropertyParameterTests
    {
        public class HasInjectionPoints
        {
            public const string PropertyName = "PropertyInjectionPoint";
            public const string WrongPropertyName = "WrongPropertyInjectionPoint";
            public const string MethodName = "MethodInjectionPoint";

            public HasInjectionPoints(string PropertyInjectionPoint) { }

            public void MethodInjectionPoint(string PropertyInjectionPoint) { }

            public string PropertyInjectionPoint { set { } }

            public string WrongPropertyInjectionPoint { set { } }
        }

        ParameterInfo GetSetAccessorParameter(PropertyInfo pi)
        {
            return pi
                .GetAccessors()
                .First()
                .GetParameters()
                .First();
        }

        ParameterInfo PropertySetValueParameter()
        {
            return GetSetAccessorParameter(
                    typeof(HasInjectionPoints)
                    .GetProperty(HasInjectionPoints.PropertyName));
                
        }

        ParameterInfo WrongPropertySetValueParameter()
        {
            return GetSetAccessorParameter(
                    typeof(HasInjectionPoints)
                    .GetProperty(HasInjectionPoints.WrongPropertyName));
        }

        ParameterInfo ConstructorParameter()
        {
            return typeof(HasInjectionPoints)
                .GetConstructors()
                .First()
                .GetParameters()
                .First();
        }

        ParameterInfo MethodParameter()
        {
            return typeof(HasInjectionPoints)
                .GetMethod(HasInjectionPoints.MethodName)
                .GetParameters()
                .First();
        }

        [Fact]
        public void MatchesPropertySetterByName()
        {
            var cp = new NamedPropertyParameter(HasInjectionPoints.PropertyName, "");
            Func<object> vp;
            Assert.True(cp.CanSupplyValue(PropertySetValueParameter(), new ContainerBuilder().Build(), out vp));
        }

        [Fact]
        public void DoesNotMatchePropertySetterWithDifferentName()
        {
            var cp = new NamedPropertyParameter(HasInjectionPoints.PropertyName, "");
            Func<object> vp;
            Assert.False(cp.CanSupplyValue(WrongPropertySetValueParameter(), new ContainerBuilder().Build(), out vp));
        }

        [Fact]
        public void DoesNotMatchConstructorParameters()
        {
            var cp = new NamedPropertyParameter(HasInjectionPoints.PropertyName, "");
            Func<object> vp;
            Assert.False(cp.CanSupplyValue(ConstructorParameter(), new ContainerBuilder().Build(), out vp));
        }

        [Fact]
        public void DoesNotMatchRegularMethodParameters()
        {
            var cp = new NamedPropertyParameter(HasInjectionPoints.PropertyName, "");
            Func<object> vp;
            Assert.False(cp.CanSupplyValue(MethodParameter(), new ContainerBuilder().Build(), out vp));
        }
    }
}
