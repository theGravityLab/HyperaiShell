pipeline {
  agent any
  stages {
    stage('Pull code') {
      steps {
        git(url: 'https://github.com/theGravityLab/HyperaiShell', branch: 'master')
      }
    }

    stage('Build') {
      steps {
        sh '''dotnet restore
dotnet build ./HyperaiShell.App/HyperaiShell.App.csproj'''
      }
    }

  }
}