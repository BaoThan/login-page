#!/bin/bash

# Define constants
REPO_URL="https://github.com/PlayFab/UnitySDK"
TARGET_FOLDER="$(pwd)/PlayfabToImport/"
PLAYFAB_SETTINGS_PATH="$(pwd)/Assets/PlayFabSdk/Shared/Public/Resources/PlayFabSharedSettings.asset"

# Function to get the latest release URL
get_latest_release_url() {
    local latest_release_url=$(curl -s -L -w "%{url_effective}" -o /dev/null "$REPO_URL/releases/latest")
    echo "$latest_release_url"
}

# Function to extract version from release URL
extract_version_from_url() {
    local release_url="$1"
    local version=$(basename "$release_url")
    echo "$version"
}

# Function to download and extract SDK
download_and_extract_sdk() {
    local sdk_version="$1"
    local sdk_download_url="$REPO_URL/archive/refs/tags/$sdk_version.zip"
    
    # Download the SDK using curl
    curl -LOk "$sdk_download_url"

    # Extract the downloaded zip file
    unzip -q "$sdk_version.zip"

    # Cleanup: remove the zip file
    rm "$sdk_version.zip"

    echo "PlayFab Unity SDK ($sdk_version) downloaded and extracted successfully."
}

# Function to copy specific files to the project folder
copy_files_and_cleanup() {
    local sdk_version="$1"
    local extracted_folder="UnitySDK-$sdk_version"
    local source_folder="$extracted_folder/Packages"

    # Create the target folder if it doesn't exist
    mkdir -p "$TARGET_FOLDER"

    # Define paths of the files to copy
    local unity_sdk_package="$source_folder/UnitySDK.unitypackage"

    # Copy the files to the target folder
    cp "$unity_sdk_package" "$TARGET_FOLDER"

    # Remove the extracted SDK folder
    rm -rf "$extracted_folder"

    echo "Files copied and extracted folder cleaned up."
}

# Function to find Unity executable dynamically
find_unity_executable() {
    local unity_exe_path=$(find /Applications -name Unity -type f -print -quit)
    if [ -z "$unity_exe_path" ]; then
        echo "Unity executable not found. Please make sure Unity is installed and try again."
        exit 1
    fi
    echo "$unity_exe_path"
}

# Function to import .unitypackage files into the Unity project
import_packages_to_unity() {
    local project_path="$(pwd)"
    local unity_exe=$(find_unity_executable)

    # Check if Unity executable exists
    if [ ! -f "$unity_exe" ]; then
        echo "Unity executable not found at $unity_exe. Please update the path to the Unity executable."
        exit 1
    fi

    # Iterate over each .unitypackage file in the target folder
    for package in "$TARGET_FOLDER"/*.unitypackage; do
        if [ -f "$package" ]; then
            echo "Importing package: $package"
            "$unity_exe" -batchmode -nographics -silent-crashes -quit -projectPath "$project_path" -importPackage "$package"
        else
            echo "No .unitypackage files found in: $TARGET_FOLDER"
        fi
    done

    echo "All packages have been imported into the Unity project."
}

# Function to set PlayFab TitleID and Secret Key
set_playfab_settings() {
    if [ ! -f "$PLAYFAB_SETTINGS_PATH" ]; then
        echo "PlayFabSharedSettings.asset not found at $PLAYFAB_SETTINGS_PATH. Please ensure the PlayFab SDK is imported correctly."
        exit 1
    fi

    # Ask user to enter Title ID and Secret Key
    read -p "Enter PlayFab Title ID: " TITLE_ID
    read -p "Enter PlayFab Secret Key: " SECRET_KEY

    # Use sed to replace titleId and developerSecretKey values
    sed -i "" "s/  TitleId: .*/  TitleId: $TITLE_ID/" "$PLAYFAB_SETTINGS_PATH"
    sed -i "" "s/  DeveloperSecretKey: .*/  DeveloperSecretKey: $SECRET_KEY/" "$PLAYFAB_SETTINGS_PATH"
    sed -i "" "s/  RequestType: .*/  RequestType: 0/" "$PLAYFAB_SETTINGS_PATH"


    echo "PlayFab settings updated with TitleID and Secret Key."
}

# Main script

# Get the latest release URL
latest_release_url=$(get_latest_release_url)

if [ -z "$latest_release_url" ]; then
    echo "Failed to fetch the latest release URL. Aborting."
    exit 1
fi

# Extract version from release URL
sdk_version=$(extract_version_from_url "$latest_release_url")

if [ -z "$sdk_version" ]; then
    echo "Failed to extract SDK version from release URL. Aborting."
    exit 1
fi

# Download and extract SDK
download_and_extract_sdk "$sdk_version"

# Copy files and clean up
copy_files_and_cleanup "$sdk_version"

# Import .unitypackage files into the Unity project
import_packages_to_unity

# Set PlayFab settings
set_playfab_settings
