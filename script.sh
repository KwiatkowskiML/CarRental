#!/bin/bash

# Function to convert gitignore patterns to find patterns
convert_gitignore_to_find_patterns() {
    local patterns=()
    while IFS= read -r line || [[ -n "$line" ]]; do
        # Skip empty lines and comments
        [[ -z "$line" || "$line" =~ ^# ]] && continue
        
        # Remove leading and trailing whitespace
        line=$(echo "$line" | sed -e 's/^[[:space:]]*//' -e 's/[[:space:]]*$//')
        
        # Skip if empty after trimming
        [ -z "$line" ] && continue
        
        # Convert pattern to find compatible pattern
        if [[ "$line" == *"/" ]]; then
            # Directory pattern (ends with /)
            pattern="-path '*/${line%/}/*'"
        elif [[ "$line" == *"/*" ]]; then
            # Directory content pattern
            pattern="-path '*/${line%/*}/*'"
        elif [[ "$line" == "/"* ]]; then
            # Absolute path pattern
            pattern="-path '*${line}'"
        elif [[ "$line" == *"/" ]]; then
            # Directory pattern
            pattern="-path '*/${line}/*'"
        elif [[ "$line" == *"*"* ]]; then
            # Pattern contains wildcard
            pattern="-path '*/${line}'"
        else
            # Simple file pattern
            pattern="-path '*/${line}'"
        fi
        
        patterns+=("-o $pattern")
    done < ".gitignore"
    
    # Join patterns with -o (OR) operator, removing the first -o
    if [ ${#patterns[@]} -gt 0 ]; then
        echo "${patterns[@]}" | sed 's/^-o //'
    fi
}

# Function to get common code file extensions
get_code_extensions() {
    echo "-name '*.js' -o -name '*.jsx' -o -name '*.ts' -o -name '*.tsx' -o -name '*.cs' -o -name '*.css' -o -name '*.scss' -o -name '*.html' -o -name '*.py' -o -name '*.java' -o -name '*.cpp' -o -name '*.h' -o -name '*.c' -o -name '*.php' -o -name '*.rb' -o -name '*.go' -o -name '*.rs' -o -name '*.swift' -o -name '*.kt' -o -name '*.xml' -o -name '*.json' -o -name '*.yaml' -o -name '*.yml' -o -name '*.sql' -o -name '*.md'"
}

# Main script
main() {
    # Check if source directory is provided
    if [ $# -ne 2 ]; then
        echo "Usage: $0 <source_directory> <destination_directory>"
        exit 1
    fi

    source_dir="$1"
    dest_dir="$2"

    # Check if source directory exists
    if [ ! -d "$source_dir" ]; then
        echo "Error: Source directory '$source_dir' does not exist"
        exit 1
    fi

    # Create destination directory if it doesn't exist
    mkdir -p "$dest_dir"

    # Check if .gitignore exists
    if [ ! -f "$source_dir/.gitignore" ]; then
        echo "Warning: .gitignore file not found in source directory"
        gitignore_patterns=""
    else
        cd "$source_dir"
        gitignore_patterns=$(convert_gitignore_to_find_patterns)
        cd - > /dev/null
    fi

    # Get code file extensions
    code_extensions=$(get_code_extensions)

    # Build the find command
    find_cmd="find \"$source_dir\" -type f \( $code_extensions \)"
    
    if [ ! -z "$gitignore_patterns" ]; then
        find_cmd="$find_cmd ! \( $gitignore_patterns \)"
    fi

    echo "Executing find command: $find_cmd"

    # Execute the find command and copy files
    while IFS= read -r file; do
        # Get the filename without path
        filename=$(basename "$file")
        
        # If file already exists in destination, append a number
        if [ -f "$dest_dir/$filename" ]; then
            counter=1
            base="${filename%.*}"
            ext="${filename##*.}"
            while [ -f "$dest_dir/${base}_${counter}.${ext}" ]; do
                ((counter++))
            done
            filename="${base}_${counter}.${ext}"
        fi
        
        # Copy the file
        cp "$file" "$dest_dir/$filename"
        echo "Copied: $file -> $dest_dir/$filename"
    done < <(eval "$find_cmd")

    echo "Operation completed successfully!"
}

# Run the main function with provided arguments
main "$@"