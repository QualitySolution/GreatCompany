/*
 * Что делает пайплайн:
 * - Собирает Windows setup (NSIS) на узле Win и сохраняет его как артефакт.
 * - При включённом Publish выкладывает setup на сервер files.qsolution.ru.
 */
properties([parameters([
	booleanParam(defaultValue: false, description: 'Выкладывать WinSetup на сервер files.qsolution.ru', name: 'Publish'),
])])

node('Win') {
	stage('Git Win') {
		checkout([
			$class: 'GitSCM',
			branches: scm.branches,
			doGenerateSubmoduleConfigurations: scm.doGenerateSubmoduleConfigurations,
			extensions: scm.extensions + [submodule(recursiveSubmodules: true)],
			userRemoteConfigs: scm.userRemoteConfigs
		])
	}
	stage('Build App') {
		bat 'dotnet publish GreatCompany\\GreatCompany.csproj --configuration Release -r win-x64 --no-self-contained'
	}
	stage('Win Setup') {
		powershell 'Remove-Item -Path "WinInstall\\Files" -Force -Recurse -ErrorAction SilentlyContinue'
		powershell 'Remove-Item -Path "WinInstall\\GreatCompany-*.exe" -Force -ErrorAction SilentlyContinue'
		powershell 'New-Item -Path "WinInstall\\Files" -ItemType Directory'
		powershell 'Copy-Item -Path "GreatCompany\\bin\\Release\\net10.0\\win-x64\\publish\\*" -Destination "WinInstall\\Files" -Recurse -Force'
		powershell '''
			chcp 65001
			& cd "WinInstall"
			& "C:\\Program Files (x86)\\NSIS\\makensis.exe" /INPUTCHARSET UTF8 "GreatCompany.nsi"
		'''
		archiveArtifacts artifacts: 'WinInstall\\GreatCompany-*.exe', onlyIfSuccessful: true
	}
	if (params.Publish) {
		stage('Publish') {
			bat 'scp WinInstall\\GreatCompany-*.exe root@odysseus.srv.qsolution.ru:/var/www/files/GreatCompany/'
		}
	}
}
