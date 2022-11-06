@if exist %temp% goto A
@if exist "%cachedir%\%9_*.wav" del "%cachedir%\%9_*.wav"
@"%resamp%" %1 %temp% %2 %vel% %flag% %5 %6 %7 %8 %params%
:A
@"%tool%" "%output%" %temp% %stp% %3 %env%
