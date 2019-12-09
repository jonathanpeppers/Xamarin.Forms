# HelloForms

This is a template I made from VS 2019 16.5.

Here are its initial performance characteristics for 10 runs for a
`Debug` build on a Pixel 3 XL:

```
Activity: Displayed     00:00:01.3940000
Activity: Displayed     00:00:01.3000000
Activity: Displayed     00:00:01.3080000
Activity: Displayed     00:00:01.2920000
Activity: Displayed     00:00:01.2460000
Activity: Displayed     00:00:01.3020000
Activity: Displayed     00:00:01.3310000
Activity: Displayed     00:00:01.3220000
Activity: Displayed     00:00:01.2740000
Activity: Displayed     00:00:01.3240000
Activity: Displayed     00:00:01.2550000
Activity: Displayed     00:00:01.3490000
Runtime.init            00:00:00.3184612
Runtime.init            00:00:00.3134054
Runtime.init            00:00:00.3159429
Runtime.init            00:00:00.3079227
Runtime.init            00:00:00.3107881
Runtime.init            00:00:00.3070668
Runtime.init            00:00:00.3115169
Runtime.init            00:00:00.3167909
Runtime.init            00:00:00.3060096
Runtime.init            00:00:00.3148874
Runtime.init            00:00:00.3054487
Runtime.init            00:00:00.3207268
----------------------------------------------------------------
Activity: Displayed     00:00:01.3080000
----------------------------------------------------------------
Runtime.init            00:00:00.3120000
----------------------------------------------------------------
```
`adb.zip` contains the full `adb logcat` output.

After removing "Anticipator":
```
Activity: Displayed     00:00:01.3630000
Activity: Displayed     00:00:01.2530000
Activity: Displayed     00:00:01.3230000
Activity: Displayed     00:00:01.2580000
Activity: Displayed     00:00:01.3130000
Activity: Displayed     00:00:01.2580000
Activity: Displayed     00:00:01.3070000
Activity: Displayed     00:00:01.3010000
Activity: Displayed     00:00:01.2630000
Activity: Displayed     00:00:01.2860000
Activity: Displayed     00:00:01.3020000
Activity: Displayed     00:00:01.2830000
Runtime.init            00:00:00.3176452
Runtime.init            00:00:00.3135985
Runtime.init            00:00:00.3238804
Runtime.init            00:00:00.3090798
Runtime.init            00:00:00.3162993
Runtime.init            00:00:00.3104849
Runtime.init            00:00:00.3199472
Runtime.init            00:00:00.3106188
Runtime.init            00:00:00.3101623
Runtime.init            00:00:00.3116972
Runtime.init            00:00:00.3156054
Runtime.init            00:00:00.3197406
----------------------------------------------------------------
Activity: Displayed     00:00:01.2930000
----------------------------------------------------------------
Runtime.init            00:00:00.3150000
----------------------------------------------------------------
```
