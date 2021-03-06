/*--------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *-------------------------------------------------------------------------------------------*/

using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Unity.Analyzers.Tests
{
	using Verify = UnityCodeFixVerifier<MessageSignatureAnalyzer, MessageSignatureCodeFix>;

	public class MessageSignatureTests : BaseTest<MessageSignatureAnalyzer, MessageSignatureCodeFix>
	{
		[Fact]
		public async Task MessageSignature()
		{
			const string test = @"
using UnityEngine;

class Camera : MonoBehaviour
{
    private void OnApplicationPause(int foo, bool pause, string[] bar)
    {
    }
}
";

			var diagnostic = Verify.Diagnostic().WithLocation(6, 18).WithArguments("OnApplicationPause");

			const string fixedTest = @"
using UnityEngine;

class Camera : MonoBehaviour
{
    private void OnApplicationPause(bool pause)
    {
    }
}
";
			await Verify.VerifyCodeFixAsync(test, diagnostic, fixedTest);
		}

		[Fact]
		public async Task MessageSignatureWithInheritance()
		{
			// two declarations for OnDestroy (one in EditorWindow and one in ScriptableObject) 
			const string test = @"
using UnityEditor;

class TestWindow : EditorWindow
{
    private void OnDestroy(int foo)
    {
    }
}
";

			var diagnostic = Verify.Diagnostic().WithLocation(6, 18).WithArguments("OnDestroy");

			const string fixedTest = @"
using UnityEditor;

class TestWindow : EditorWindow
{
    private void OnDestroy()
    {
    }
}
";
			await Verify.VerifyCodeFixAsync(test, diagnostic, fixedTest);
		}

	}
}
