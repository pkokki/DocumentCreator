import * as React from "react";

export interface HeaderProps {
  title: string;
  logo: string;
}

const Header: React.FC<HeaderProps> = (props: HeaderProps) => {
  const { title, logo } = props;
  return (
    <section className="ms-welcome__header ms-bgColor-neutralLighter ms-u-fadeIn500">
      <img width="60" height="60" src={logo} alt={title} title={title} />
    </section>
  );
};

export default Header;
