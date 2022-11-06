@rem project=New Project
@set loadmodule=
@set tempo=120
@set samples=44100
@set oto={{oto}}
@set tool={{tool}}
@set resamp={{resampler}}
@set output=temp.wav
@set helper=temp_helper.bat
@set cachedir={{cachedir}}
@set flag="{{flag}}"
@set env=0 5 35 0 100 100 0
@set stp=0

@del "%output%" 2>nul
@mkdir "%cachedir%" 2>nul

@set params=100 0 !120 AA#8#
@set env=0 5 35 0 100 100 0 18
@set vel=100
@set temp="%cachedir%\{{character}}.wav"
@echo ########################################(1/1)
@call %helper% "%oto%\{{character}}.wav" {{key}} {{length}}@120+44.0 {{preUtterance}} {{leftBlank}} 1100 {{consonant}} {{rightBlank}} 0

@if not exist "%output%.whd" goto E
@if not exist "%output%.dat" goto E
copy /Y "%output%.whd" /B + "%output%.dat" /B "%output%"
del "%output%.whd"
del "%output%.dat"
:E
