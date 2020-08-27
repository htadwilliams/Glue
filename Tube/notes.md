# Random developer notes

## Adding a new action

* Add entry for new ActionType enum in Action.cs
* Extend class Action with implementations of Play() and Schedule() methods. 
* Make sure new class sets action type correctly in its constructor.
* Add case for new ActionType in factory method ActionConverter::ReadJson().
* Add example use to DefaultContent.cs.
* Verify serialization / deserialization works for new action. Run with DEFAULT command-line parameter to force generation of macros.glue and verify contents after exiting. Run again with NO command-line parameter and verify macro works after deserialization.

## Related projects 

https://github.com/sharpdx/SharpDX/issues/979

https://github.com/evilC/IOWrapper  (evilc@evilc.com)

## Keyboard Interceptor driver

http://www.oblita.com/interception.html

https://github.com/oblitum/Interception

https://github.com/jasonpang/Interceptor

The code injects extra information that may be able to be detected and used to prevent dreaded remap / trigger infinite loops. 

see https://github.com/oblitum/Interception/blob/master/library/interception.c line 205 (commit 5fb12fc)

    rawstrokes[i].ExtraInformation = key_stroke[i].information;) 

## Other interesting links

Seeking advice on best way to gracefully handle device unplug / replug in DirectInput #979

https://github.com/sharpdx/SharpDX/issues/979

## Github 

### Renaming a branch

https://stackoverflow.com/questions/9524933/renaming-a-branch-in-github

#### Rename local branch
```
git branch -m oldname newname
```

#### Push branch with new name while deleting old branch
```
git push origin :oldname newname
```

