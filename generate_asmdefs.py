import os
import json

# Define the path to your Scripts folder
scripts_dir = os.path.join("Assets", "Scripts")

# The core assembly we need to reference
core_assembly_name = "MutedMelody.Core"

# Check if the Scripts directory exists
if not os.path.exists(scripts_dir):
    print(f"❌ Error: Could not find '{scripts_dir}'. Make sure you run this from the Unity project root.")
    exit(1)

# Iterate through every folder inside Assets/Scripts
for folder_name in os.listdir(scripts_dir):
    folder_path = os.path.join(scripts_dir, folder_name)

    # We only care about directories, not files
    if os.path.isdir(folder_path):
        asmdef_filename = f"MutedMelody.{folder_name}.asmdef"
        asmdef_filepath = os.path.join(folder_path, asmdef_filename)

        # Skip if an .asmdef already exists in this folder (e.g., Core and Player)
        existing_asmdefs = [f for f in os.listdir(folder_path) if f.endswith('.asmdef')]
        if existing_asmdefs:
            print(f"⏩ Skipping '{folder_name}' — Already contains {existing_asmdefs[0]}")
            continue

        # Build the exact JSON structure Unity expects for an Assembly Definition
        asmdef_data = {
            "name": f"MutedMelody.{folder_name}",
            "rootNamespace": f"MutedMelody.{folder_name}",
            "references": [
                core_assembly_name
            ],
            "includePlatforms": [],
            "excludePlatforms": [],
            "allowUnsafeCode": False,
            "overrideReferences": False,
            "precompiledReferences": [],
            "autoReferenced": True,
            "defineConstraints": [],
            "versionDefines": [],
            "noEngineReferences": False
        }

        # Write the JSON data to the new .asmdef file
        with open(asmdef_filepath, 'w', encoding='utf-8') as f:
            json.dump(asmdef_data, f, indent=4)

        print(f"✅ Created: {asmdef_filename} (Referencing: {core_assembly_name})")

print("\n🚀 All missing Assembly Definitions have been generated!")
