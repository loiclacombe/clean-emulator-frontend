namespace csharp CleanEmulatorFrontend.Cache.Data

struct PersistedEmulatedSystems {
	1: i32 version = 100,
	2: list<EmulatedSystem> emulatedSystemsData
}

struct EmulatedSystem{
	1: string ShortName,
	2: list<Game> Games
}

struct Game{
    1: string Description,
    2: string BasePath,
    3: string LaunchPath,
	4: string Guid
}