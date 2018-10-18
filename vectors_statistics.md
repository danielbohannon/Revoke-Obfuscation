# Vectors Statistics

## Default

'highConfidenceWeightedVector' is being used:  

Weighted feature vector generated from ModelTrainer.cs/ModelTrainer.exe during the training phase based on in-the-wild samples.  

- Accuracy: 0.9605  
- Precision: 0.9609  
- Recall: 0.9353  
- F1Score: 0.9479  
- TruePositiveRate: 0.3595  
- FalsePositiveRate: 0.0146  
- TrueNegativeRate: 0.6010  
- FalseNegativeRate: 0.0249  

## -Deep

*Specifies that the deeper (but lower confidence) weighted vector be used to measure input vector, thus a "deep" inspection that will allow more False Positives but fewer False Negatives than the default high confidence weighted vector.*

'broadNetWeightedVector' is being used:  

Weighted feature vector generated from ModelTrainer.cs/ModelTrainer.exe during the training phase.

- Accuracy: 0.8926
- Precision: 0.8977
- Recall: 0.8849
- F1Score: 0.8913
- TruePositiveRate: 0.4403
- FalsePositiveRate: 0.0502
- TrueNegativeRate: 0.4523
- FalseNegativeRate: 0.0573

## -CommandLine

*Specifies that the command-specific (as opposed to the default script-specific) weighted vector be used to measure input vector.*

'commandLineWeightedVector' is being used:  

Weighted feature vector generated from ModelTrainer.cs/ModelTrainer.exe during the training phase based on in-the-wild samples.  
  
- Accuracy: 0.9959  
- Precision: 0.9937  
- Recall: 0.9918  
- F1Score: 0.9927  
- TruePositiveRate: 0.2789  
- FalsePositiveRate: 0.0018  
- TrueNegativeRate: 0.7170  
- FalseNegativeRate: 0.0023  

## -Normalized

*Specifies that only normalized and important features be used to measure obfuscation probablity.*

'normalizedWeightedVector' is being used:  

Weighted feature vector generated from ModelTrainer.cs/ModelTrainer.exe after some of the training changes like:

1. Statistical data binning;
    *Number of bins for each feature = 100*
2. Pruning of irrelevant features;
    *Important features are the ones where number of unique bins > 3*
3. Randomization of the data set;
4. Use of development dataset;
5. Early stopping to prevent overfitting;

Best obtained performance is:

- Accuracy: 0.9154
- Precision: 0.9275
- Recall: 0.8958
- F1Score: 0.9114
- TruePositiveRate: 0.4351
- FalsePositiveRate: 0.0340
- TrueNegativeRate: 0.4802
- FalseNegativeRate: 0.0506