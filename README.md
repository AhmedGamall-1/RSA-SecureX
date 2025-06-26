# RSA SecureX

This is an implementation of the RSA public-key cryptosystem using a custom BigInteger class that can handle large integers (hundreds of digits).

## Features

- Custom BigInteger implementation supporting:
  - Addition
  - Subtraction
  - Multiplication
  - Division
  - Modular operations
- RSA encryption and decryption
- Efficient modular exponentiation
- Execution time measurement
- String support (Bonus Level 1)
- RSA key generation (Bonus Level 2)

## Input Format

The program expects an input file with the following format:
1. First line: Number of test cases (N)
2. For each test case, 4 lines:
   - N (modulus)
   - e or d (public/private key)
   - M or E(M) (message or encrypted message)
   - 0 or 1 (0 for encryption, 1 for decryption)

## Usage

1. Compile the program:
```bash
dotnet build
```

2. Run the program with an input file:
```bash
dotnet run <input_file>
```

3. Generate RSA keys:
```bash
dotnet run -- --generate-keys <bit_length>
```

The results will be written to `output.txt` and execution times will be displayed in the console.

## Example Input

### Basic Usage
```
1
3713
7
2003
0
```

This will encrypt the message 2003 using the public key (7, 3713).

### String Encryption
```
1
3713
7
Hello, World!
0
```

This will encrypt the string "Hello, World!" using the public key (7, 3713).

## Output

The program creates an `output.txt` file containing the results of each operation, one per line. The console output will show the results along with execution times for each test case.

## Bonus Features

### Level 1: String Support
The program can now handle string input and output:
- Strings are converted to integers using UTF-8 encoding
- Encrypted strings are represented as hexadecimal numbers
- Decrypted output is automatically converted back to the original string

### Level 2: Key Generation
The program can generate RSA keys:
- Uses Miller-Rabin primality test for generating large prime numbers
- Implements efficient key generation with configurable bit length
- Finds a suitable public exponent e that is relatively prime to φ(n)
- Outputs the public key (n, e) for use in encryption
