// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle
{
	using System;

	using Castle.DynamicProxy;
	using Castle.Interceptors;

	using NUnit.Framework;

	[TestFixture]
	public class RetryInterceptorTests
	{
		readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void WillThrowAnExceptionWithOnlyOneRetry()
		{
			var executor = new Executor(3, new InvalidOperationException());
			var proxy = (Executor)proxyGenerator.CreateClassProxyWithTarget(typeof(Executor), executor, new[] { new RetryInterceptor(1) });
			proxy.Execute();
			Assert.Fail();
		}

		[Test]
		public void WillBeSuccessfulOnThirdTry()
		{
			var executor = new Executor(3, new InvalidOperationException());
			var proxy = (Executor)proxyGenerator.CreateClassProxyWithTarget(typeof(Executor), executor, new[] { new RetryInterceptor(3) });
			proxy.Execute();
		}

		public class Executor
		{
			private readonly int suceedOnAttempt;
			private readonly Exception e;
			private int attempts;

			public Executor()
			{
			}

			public Executor(int suceedOnAttempt, Exception e)
			{
				this.suceedOnAttempt = suceedOnAttempt;
				this.e = e;
			}

			public virtual void Execute()
			{
				attempts = attempts + 1;
				if (attempts < suceedOnAttempt) throw e;
			}
		}
	}
}