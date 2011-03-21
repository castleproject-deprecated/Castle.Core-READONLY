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

namespace Castle.Interceptors
{
	using System;
	using System.Collections.Generic;

	using Castle.DynamicProxy;

	public class RetryInterceptor : IInterceptor
	{
		private readonly int numberOfRetries;

		public RetryInterceptor(int numberOfRetries)
		{
			this.numberOfRetries = numberOfRetries;
		}

		public void Intercept(IInvocation invocation)
		{
			var attempts = 0;
			var exceptions = new List<Exception>();

			Exception exception = null;
			while (attempts < numberOfRetries)
			{
				bool failed = TryExecute(invocation, out exception);
				if (!failed) return;
				exceptions.Add(exception);
				attempts++;
			}

			throw new InvalidOperationException("Retries failed, last exception: " + exception.Message, exception);
		}

		private static bool TryExecute(IInvocation invocation, out Exception exception)
		{
			exception = null;

			var failed = false;
			try
			{
				invocation.Proceed();
			} 
			catch (Exception e)
			{
				invocation.Rollback();
				exception = e;
				failed = true;
			}
			return failed;
		}
	}
}
