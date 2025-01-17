import os

def print_sql_files(directory='sql/tables'):
    """
    Iterate over SQL files in the specified directory and print their contents.
    
    Args:
    directory (str): The directory containing SQL files. Defaults to 'sql/tables'.
    """
    # Ensure the directory exists
    if not os.path.exists(directory):
        print(f"Error: Directory '{directory}' does not exist.")
        return

    # Iterate over files in the directory
    for filename in os.listdir(directory):
        if filename.endswith('.sql'):
            file_path = os.path.join(directory, filename)
            
            try:
                # Open and read the file
                with open(file_path, 'r') as file:
                    content = file.read()
                    print(content)
            except IOError as e:
                print(f"Error reading file {filename}: {e}")

if __name__ == "__main__":
    print_sql_files()