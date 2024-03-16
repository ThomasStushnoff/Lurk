import os
import re
import argparse

def print_warning(message):
    print(f"\033[93m[WARNING]\033[0m " + message)

def create_audio_data(input_dir, output_dir, mixer_group):
    supported_extensions = ['.wav', '.aif', '.mp3', '.ogg']
    asset_content_template = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: 0e4753300ba24741a222eef7dcdfd544, type: 3}}
  m_Name: {file_name}
  m_EditorClassIdentifier: 
  clip: {{fileID: 8300000, guid: {guid}, type: 3}}
  mixerGroup: {mixer_group}
  playOnAwake: 0
  loop: 0
  volume: 1
  spatialize: 0
    """

    for filename in os.listdir(input_dir):
        if any(filename.endswith(ext) for ext in supported_extensions):
            meta_filename = filename + ".meta"
            meta_file_path = os.path.join(input_dir, meta_filename)

            if os.path.exists(meta_file_path):
                with open(meta_file_path, 'r') as meta_file:
                    meta_content = meta_file.read()
                    match = re.search(r'guid: ([a-z0-9]+)', meta_content)
                    if match:
                        guid = match.group(1)

                        file_base_name = filename.split('.')[0]
                        asset_content = asset_content_template.format(file_name=file_base_name, guid=guid, mixer_group=mixer_group)
                        
                        asset_file_path = os.path.join(output_dir, filename.replace(os.path.splitext(filename)[1], ".asset"))
                        os.makedirs(output_dir, exist_ok=True)
                        with open(asset_file_path, 'w') as asset_file:
                            asset_file.write(asset_content)
            else:
                print_warning(f"No .meta file found for {filename}.\nInitialize meta files for all audio files by opening Unity.")
                return

def main():
    parser = argparse.ArgumentParser(description='Generate audio data for Lurk and set the mixer group')
    parser.add_argument('--music', action='store_true', help='Set the mixer group to Music')
    parser.add_argument('--sfx', action='store_true', help='Set the mixer group to SoundFx')
    parser.add_argument('--voice', action='store_true', help='Set the mixer group to VoiceOver')

    args, unknown = parser.parse_known_args()
    if unknown:
        parser.print_help()
        return

    if args.music:
        mixer_group = "{fileID: -6156782006384238876, guid: 26c0ca923bdf63b4bb915eb3697b7c0c, type: 2}"
    elif args.sfx:
        mixer_group = "{fileID: -1267447417658469395, guid: 26c0ca923bdf63b4bb915eb3697b7c0c, type: 2}"
    elif args.voice:
        mixer_group = "{fileID: 7500764252588437308, guid: 26c0ca923bdf63b4bb915eb3697b7c0c, type: 2}"
    else:
        mixer_group = "{fileID: 0}"

    input_path = input("Path to audio files (supports .wav, .aif, .mp3, .ogg): ")
    output_path = input("Path to output directory: ")
    create_audio_data(input_path, output_path, mixer_group)

if __name__ == "__main__":
    main()