node {

    def mvnHome
    def commitId
    properties([gitLabConnection('GitLab')])
    
    stage('preparation') { 
        checkout scm
        commitId = sh(returnStdout: true, script: 'git rev-parse HEAD')
        updateGitlabCommitStatus name: 'restore', state: 'pending', sha: commitId
        updateGitlabCommitStatus name: 'build', state: 'pending', sha: commitId
        updateGitlabCommitStatus name: 'publish', state: 'pending', sha: commitId
        updateGitlabCommitStatus name: 'test', state: 'pending', sha: commitId
        if(env.BRANCH_NAME == 'master'){
            updateGitlabCommitStatus name: 'containerize', state: 'pending', sha: commitId
        }
        updateGitlabCommitStatus name: 'clean', state: 'pending', sha: commitId
    }

    try{
        stage('restore') {
            gitlabCommitStatus("restore") {
                sh 'dotnet restore'
            }
        }
    }catch(Exception ex){
        updateGitlabCommitStatus name: 'restore', state: 'failed', sha: commitId
        updateGitlabCommitStatus name: 'build', state: 'canceled', sha: commitId
        updateGitlabCommitStatus name: 'publish', state: 'canceled', sha: commitId
        updateGitlabCommitStatus name: 'test', state: 'canceled', sha: commitId
        if(env.BRANCH_NAME == 'master'){
            updateGitlabCommitStatus name: 'containerize', state: 'canceled', sha: commitId
        }
        updateGitlabCommitStatus name: 'clean', state: 'canceled', sha: commitId
        currentBuild.result = 'FAILURE'
        echo "RESULT: ${currentBuild.result}"
        return 
    }
    
    try{
        stage('build'){
            gitlabCommitStatus("build") {
                sh 'dotnet build'
            }
        }
    }catch(Exception ex){
        updateGitlabCommitStatus name: 'build', state: 'failed', sha: commitId
        updateGitlabCommitStatus name: 'publish', state: 'canceled', sha: commitId
        updateGitlabCommitStatus name: 'test', state: 'canceled', sha: commitId
        if(env.BRANCH_NAME == 'master'){
            updateGitlabCommitStatus name: 'containerize', state: 'canceled', sha: commitId
        }
        updateGitlabCommitStatus name: 'clean', state: 'canceled', sha: commitId
        currentBuild.result = 'FAILURE'
        echo "RESULT: ${currentBuild.result}"
        return
    }

    try{
        stage('publish'){
            gitlabCommitStatus("publish") {
                sh 'dotnet publish -c Release'
            }
        }
    }catch(Exception ex){
        updateGitlabCommitStatus name: 'publish', state: 'failed', sha: commitId
        updateGitlabCommitStatus name: 'test', state: 'canceled', sha: commitId
        if(env.BRANCH_NAME == 'master'){
            updateGitlabCommitStatus name: 'containerize', state: 'canceled', sha: commitId
        }
        updateGitlabCommitStatus name: 'clean', state: 'canceled', sha: commitId
        currentBuild.result = 'FAILURE'
        echo "RESULT: ${currentBuild.result}"
        return
    }

    try{
        stage('test') {
            gitlabCommitStatus("test") {
                sh 'dotnet test' // /p:CollectCoverage=true /p:Include="[Website.Nuqneh.de]*"'
            }
        }
    }catch(Exception ex){
        updateGitlabCommitStatus name: 'test', state: 'failed', sha: commitId
        if(env.BRANCH_NAME == 'master'){
            updateGitlabCommitStatus name: 'containerize', state: 'canceled', sha: commitId
        }
        updateGitlabCommitStatus name: 'clean', state: 'canceled', sha: commitId
        currentBuild.result = 'FAILURE'
        echo "RESULT: ${currentBuild.result}"
        return
    }

    try{
        if(env.BRANCH_NAME == 'master'){
            stage('containerize'){
                gitlabCommitStatus("containerize") {
                    mvnHome = env.BUILD_NUMBER
                    sh "docker build -t docker.qaybe.de/stack.air:0.0.${mvnHome} ."
                    withDockerRegistry(credentialsId: 'DockerRegistry', toolName: 'QaybeDocker', url: 'https://docker.qaybe.de') {
                        sh "docker push docker.qaybe.de/stack.air:0.0.${mvnHome}"   
                    }
                }
            }
        }
    }catch(Exception ex){
        updateGitlabCommitStatus name: 'containerize', state: 'failed', sha: commitId
        updateGitlabCommitStatus name: 'clean', state: 'canceled', sha: commitId
        currentBuild.result = 'FAILURE'
        echo "RESULT: ${currentBuild.result}"
        return
    }
    
 try{
     stage('clean'){
         gitlabCommitStatus("clean") {
             cleanWs()
         }
     }
    }catch(Exception ex){
        updateGitlabCommitStatus name: 'clean', state: 'failed', sha: commitId
        currentBuild.result = 'FAILURE'
        echo "RESULT: ${currentBuild.result}"
        return
    }
}
