# GithubWebhookListener

Small project to listen to github webhooks and update builds for local CI/CD.

For running on arm linux recommend using `dotnet publish -c release -r linux-arm64 --self-contained`
Then copying the output directory. `chmod 777 ./GithubWebhookListener` on the target machine.
Then `pm2 start ./GithubWebhookListener --name github_webhooks`
