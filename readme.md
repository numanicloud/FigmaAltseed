# FigmaAltseed

## プロジェクト群

|プロジェクト名|役割|
|-|-|
|FigmaVisk|FigmaのレイアウトをVisklusa形式に変換するプロジェクトです。|
|FigmaVisk.Capability|FigmaViskで用いるCapability群を定義するプロジェクトです。|
|ViskAltseed2.Packer|Visklusa(JsonZip)形式のデータをAltseed2向けに変換するプロジェクトです。|
|Visklusa.JsonAltseed|VisklusaのAltseed2向け形式を定義するプロジェクトです。|
|VisklusaVectorRenderer|Visklusaでーたに含まれるベクタグラフィックスの情報をラスタリングするプロジェクトです。|
|FigmaAltseed|古いプロジェクトです。FigmaAltseed2を使用してください。|
|FigmaAltseed.Common|古いプロジェクトです。|
|FigmaAltseed2|以上のプロジェクトを利用して、Figmaを読み込んでAltseed2向け形式へ変換するまでを実行するプロジェクトです。
|FigmaAltseed2.Try|VisklusaのAltseed2形式の読み込みを試すプロジェクトです。|
|ViskAltseed2|VisklusaのAltseed2形式を読み込み、Altseed2のノードを実際に生成するプロジェクトです。|

## ファイル形式

Visklusa形式は`visk`で始まる6文字の拡張子を持つファイルで取りまわすのが基本です。

例として、レイアウト情報をJSONで表し、アセット群をZipでまとめた形式の場合、
それを「Visklusa(JsonZip)形式」などと呼び、拡張子 `viskjz` を用います。

このプロジェクトではAltseed2向けの形式があるので、そちらは `viskja` という形式で扱っています。
