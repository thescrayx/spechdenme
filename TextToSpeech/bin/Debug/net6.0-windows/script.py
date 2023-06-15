import sys

if __name__ == "__main__":
    if len(sys.argv) > 1:
        text = sys.argv[1]
        print("Received text:", text)
    else:
        print("No text provided.")
