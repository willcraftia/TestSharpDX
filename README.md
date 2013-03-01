TestSharpDX
==========

# 概要

プロジェクト名 Libra。

C# で DirectX を利用するためのフレームワークである SharpDX のラッパー。

Direct3D 11.0 対応のみ。

# Libra アセンブリ

ベクトル構造体や行列構造体などの基本コードを提供するアセンブリ。

SharpDX には既にそれらが含まれているが、それらは DirectX から独立させられるため、
SharpDX への依存性を下げる目的で SharpDX より移植。
なお、汎用性を必要としないため、機能の多くを削除している
(例えば、左手系と右手系の両方への対応などは不要である、など)。

# Libra.Graphics アセンブリ

SharpDX のラッパー本体であり、Direct3D11 へのアクセスを担うアセンブリ。

# Libra.Compiler アセンブリ

SharpDX.D3DCompiler を用いた簡易シェーダ コンパイラのアセンブリ。

# Libra.Games アセンブリ

ゲーム開発のためのクラスを提供するアセンブリ。

SharpDX.Toolkit は SharpDX への依存が強すぎるため、これを用いずに独自の実装を用いる。

# Libra.Samples.XXX アセンブリ

各アセンブリは、動作確認のためのサンプル アプリケーションに対応。