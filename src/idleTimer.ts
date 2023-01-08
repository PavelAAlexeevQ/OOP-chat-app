export class IdleTimer {
  private timeout: NodeJS.Timeout;
  private timeoutCallback: () => void;
  private timeoutTime: number;

  constructor(fn: () => void, timeoutValue: number) {
    this.timeout = setTimeout(fn, timeoutValue);
    this.timeoutCallback = fn;
    this.timeoutTime = timeoutValue;
  }

  public resetTimeout = () => {
    clearTimeout(this.timeout);
    this.timeout = setTimeout(this.timeoutCallback, this.timeoutTime);
  };
}
