export class BloodGroupUtil {
  private static readonly BLOOD_GROUP_MAP: { [key: number]: string } = {
    0: 'APositive',
    1: 'ANegative', 
    2: 'BPositive',
    3: 'BNegative',
    4: 'ABPositive',
    5: 'ABNegative',
    6: 'OPositive',
    7: 'ONegative'
  };

  private static readonly DISPLAY_MAP: { [key: string]: string } = {
    'APositive': 'A+',
    'ANegative': 'A-',
    'BPositive': 'B+', 
    'BNegative': 'B-',
    'ABPositive': 'AB+',
    'ABNegative': 'AB-',
    'OPositive': 'O+',
    'ONegative': 'O-'
  };

  private static readonly REVERSE_DISPLAY_MAP: { [key: string]: string } = {
    'A+': 'APositive',
    'A-': 'ANegative',
    'B+': 'BPositive',
    'B-': 'BNegative', 
    'AB+': 'ABPositive',
    'AB-': 'ABNegative',
    'O+': 'OPositive',
    'O-': 'ONegative'
  };

  static getDisplayValue(bloodGroup: number | string | null | undefined): string {
    if (bloodGroup === null || bloodGroup === undefined) return 'N/A';
    
    if (typeof bloodGroup === 'number') {
      const enumName = this.BLOOD_GROUP_MAP[bloodGroup];
      return enumName ? this.DISPLAY_MAP[enumName] || 'N/A' : 'N/A';
    }
    
    if (typeof bloodGroup === 'string') {
      return this.DISPLAY_MAP[bloodGroup] || 'N/A';
    }
    
    return 'N/A';
  }

  static getEnumValue(displayValue: string): number | null {
    const enumName = this.REVERSE_DISPLAY_MAP[displayValue];
    if (!enumName) return null;
    
    for (const [key, value] of Object.entries(this.BLOOD_GROUP_MAP)) {
      if (value === enumName) {
        return parseInt(key);
      }
    }
    return null;
  }

  static getEnumName(bloodGroup: number | null | undefined): string | null {
    if (bloodGroup === null || bloodGroup === undefined) return null;
    return this.BLOOD_GROUP_MAP[bloodGroup] || null;
  }

  static getBloodGroupOptions() {
    return [
      { value: 0, label: 'A+', enumName: 'APositive' },
      { value: 1, label: 'A-', enumName: 'ANegative' },
      { value: 2, label: 'B+', enumName: 'BPositive' },
      { value: 3, label: 'B-', enumName: 'BNegative' },
      { value: 4, label: 'AB+', enumName: 'ABPositive' },
      { value: 5, label: 'AB-', enumName: 'ABNegative' },
      { value: 6, label: 'O+', enumName: 'OPositive' },
      { value: 7, label: 'O-', enumName: 'ONegative' }
    ];
  }
}