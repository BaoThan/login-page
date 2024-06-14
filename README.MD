## Development Setup
### Requirements
- **TitleID**: Your PlayFab TitleID (https://developer.playfab.com/en-us/my-games)
- **Secret Key**: Log in to PlayFab, select your title, navigate to **Settings > API Features** in the Game Manager, and find your secret key there.

### About this Script
- Downloads the latest PlayFab SDK.
- Prompts you to enter TitleID and SecretKey.

```bash
$ chmod +x setup.sh && ./setup.sh
```

### On error:
#### Failed Import of PlayFab SDK:
- Manually import with the project open: Assets > Import Package > Custom Package...
- Navigate to current project folder > PlayfabToImport > UnitySDK.unitypackage.

#### Failed Download:
- Download the latest .zip from: https://github.com/PlayFab/UnitySDK/releases/latest
- Extract and import the package: extracted folder > Packages > UnitySDK.unitypackage.

#### Setup PlayFab SDK After Import:
1. Open your project.
2. Go to: Assets > PlayFabSDK > Shared > Public > Resources > PlayFabSharedSettings.
3. In the Inspector:
- TitleID: 'Your TitleID'
- Developer Secret Key: 'Your Secret Key'
- Request Type: Unity Web Request
